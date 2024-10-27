using Newtonsoft.Json;
using RenSharp.Core.Exceptions;
using RenSharp.Core.Parse;
using RenSharp.Core.Repositories;
using RenSharp.Core.Save;
using RenSharp.Interfaces;
using RenSharp.Models;
using RenSharp.Models.Callback;
using RenSharp.Models.Commands;
using RenSharp.Models.Save;
using RenSharp.Core.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Assets.Scripts.RenSharp.Core;

namespace RenSharp.Core
{
    public class RenSharpCore
	{
		public bool _isPaused { get; private set; } = false;
		// TODO: add NopSavePreviousCommand
		// TODO: execute on LOAD command queue
		private  Stack<Func<RenSharpCore, PausePredicateAction>> PausePredicates { get; set; } = new Stack<Func<RenSharpCore, PausePredicateAction>>();

		public Mutex Mutex { get; private set; } = new Mutex();

		public Configuration Configuration { get; set; }
		public RenSharpContext Context { get; set; }
		private RenSharpProgram Program => Context.Program;
		private bool HasStarted { get; set; } = false;
		private CharacterRepository CharacterRepository { get; set; }

		private Stack<RenSharpCallback> InsteadNextCommandCallbacks { get; set; } = new Stack<RenSharpCallback>();
        private Stack<RenSharpCallback> Callbacks { get; set; } = new Stack<RenSharpCallback>();
        private Stack<(RenSharpCallback, Func<bool>)> InsteadNextCommandIfPredicateCallbacks { get; set; } = new Stack<(RenSharpCallback, Func<bool>)>();

        public RenSharpCore(string path, Configuration config = null) => SetupProgramWithCode(File.ReadAllLines(path), config);
        public RenSharpCore(IEnumerable<string> code, Configuration config = null) => SetupProgramWithCode(code, config);
		public RenSharpCore(Configuration config = null)
		{
			SetConfig(config);
            SetupProgram();
		}

		private void SetupProgramWithCode(IEnumerable<string> code, Configuration config)
		{
			SetConfig(config);
            SetupProgram();
            LoadProgram(code, saveScope: true);
        }

		private void SetupProgram()
		{
            PyImportAttribute.ReloadCallbacks();
            Context = new RenSharpContext();
            CharacterRepository = new CharacterRepository();

			string defaultName = Configuration.GetValueOrDefault<string>("default_character_name");
            string key = CharacterRepository.AddCharacter(new Character(name: defaultName));

            Context.SetVariable("rs", this);
            Context.SetVariable("_rs_nobody_character", key);
        }

		private void SetConfig(Configuration config)
		{
            if (config == null)
                Configuration = DefaultConfiguration.GetDefaultConfig();
            else
                Configuration = config;
        }

		public void LoadProgram(string path, bool saveScope = false)
			=> LoadProgram(File.ReadAllLines(path), saveScope);
		public void LoadProgram(IEnumerable<string> code, bool saveScope = false)
		{
			var parser = new RenSharpParser(Configuration);
			var program = parser.ParseCode(code);

			LoadProgram(program, saveScope);
		}

		public void LoadProgram(List<Command> program, bool saveScope = false)
		{
			if (saveScope == false)
			{
				Context.PyEvaluator.RecreateScope();
				SetupProgram();
			}

			Context.Program = new RenSharpProgram(program);
			HasStarted = false;
		}

		public SaveModel SaveRaw()
		{
			var save = new SaveModel()
			{
				IsPaused = IsPaused(),
				HasStarted = HasStarted,
				Line = Program.Line,
				MessageHistory = Context.MessageHistory.Messages,
				CallStack = Context.CallStack,
				RollbackStack = Context.RollbackStack,
				CurrentFrame = Context.CurrentFrame,
				Variables = Context.PyEvaluator.GetVariables()
			};
			return save;
		}

		public string SaveJson()
		{
			SaveModel save = SaveRaw();
			string serialized = JsonConvert.SerializeObject(save);
			return serialized;
		}

		public void Load(string serializedSave)
		{
			SaveModelJson savePreParsed = JsonConvert.DeserializeObject<SaveModelJson>(serializedSave);

			List<Command> RollbackStack = new List<Command>();

			foreach(object commandJObject in savePreParsed.RollbackStack)
			{
				string commandJson = commandJObject.ToString();
				string commandTypeName = JsonConvert.DeserializeObject<CommandTypeJson>(commandJson).TypeName;

				Func<string, RenSharpCore, Command> parser;
                bool hasParser = Configuration.DeserializeParsers.TryGetValue(commandTypeName, out parser);


                if (hasParser == false || parser == null)
					throw new ArgumentException($"По значению '{commandTypeName}' указан не валидный парсер.");

				Command command = null;

				try
				{
                    command = parser(commandJson, this);
                }
				catch
				{
					throw new ArgumentException("Can not parse command from save.");
                }

                if (command == null)
					throw new ArgumentException("Can not parse command from save.");
				else
				{
                    RollbackStack.Add(command);
					continue;
                }

            }

			var save = new SaveModel(savePreParsed, RollbackStack);
            Load(save);
        }

		public void Load(SaveModel save)
		{
			SetConfig(Configuration);
			SetupProgram();
			Context.PyEvaluator.RecreateScope();

			// If save should be start
			if (save.HasStarted)
			{
				HasStarted = false;
				Start();
			}

			Program.Goto(save.Line);
			_isPaused = save.IsPaused;

			Context.Load(save);
		}

		public void AddCallbackInsteadNextCommand(RenSharpCallback callback)
			=> InsteadNextCommandCallbacks.Push(callback);

		public void AddCallback(RenSharpCallback callback)
			=> Callbacks.Push(callback);

		public void AddInsteadNextCommandIfPredicateCallbacks(RenSharpCallback callback, Func<bool> predicate)

			=> InsteadNextCommandIfPredicateCallbacks.Push((callback, predicate));


        public string AddCharacter(string name)
		{
			var character = new Character(name);
			string key = CharacterRepository.AddCharacter(character);
			return key;
		}

		public bool Rollback()
		{
			Command command;

			while(true)
			{
                bool hasRollback = Context.RollbackStack.TryPop(out command);
				if (hasRollback == false)
					return false;

                ClearCallbacksWithoutCallIfRollbackUsed();

                Goto(command);

				command.Execute(this);

				if (Configuration.IsNotSkip(command))
					break;
			}
			return true;
		}

		private void ClearCallbacksWithoutCallIfRollbackUsed()
		{
			ClearCallbacksWithoutCallIfRollbackUsed(Callbacks);
			ClearCallbacksWithoutCallIfRollbackUsed(InsteadNextCommandCallbacks);
			ClearCallbacksWithoutCallIfRollbackUsed(InsteadNextCommandIfPredicateCallbacks.Select(x => x.Item1));

        }

		private void ClearCallbacksWithoutCallIfRollbackUsed(IEnumerable<RenSharpCallback> callbacks)
		{
			callbacks = callbacks.Where(x => x.CallIfRollbackUsed).ToList();
		}


		/// <summary>
		/// Запускает все блоки init и переводит курсор на блок 'start'
		/// </summary>
		/// <exception cref="Exception">Если label 'start' не найден</exception>
		/// <exception cref="ArgumentException">Если label 'start' имеет табы</exception>
		public void Start()
		{
			if (HasStarted)
				return;
			HasStarted = true;

			ExecuteInits();
			Label main = Program.GetLabel("start");
			Goto(main);
		}

        public Command ReadNext(bool force = false)
        {
            try
			{
				bool isMutexFree = Mutex.WaitOne(millisecondsTimeout: 1000);
				if (isMutexFree == false)
					throw new RenSharpLockedException("RenSharp заблокирован. Скорее всего идет выполнение другой команды.");

                if (!HasStarted)
                    Start();
                if (IsPaused() && force == false)
                    throw new RenSharpPausedException("RenSharp находится на паузе.");

                bool containsInsteadNextCommandCallback = ExecuteCallbacks();

                if (containsInsteadNextCommandCallback)
					return new Nop("Callbacks executed instead of command.");

                Command command = ExecuteNext();
                return command;
            }
            catch { throw; }
			finally
			{
				Mutex.ReleaseMutex();
			}
        }

		private bool ExecuteCallbacks()
		{
			bool containsInsteadNextCommandCallback = false;

			while(InsteadNextCommandCallbacks.Any())
			{
				InsteadNextCommandCallbacks.Pop()?.Callback?.Invoke();
				containsInsteadNextCommandCallback = true;
            }

			while(Callbacks.Any())
			{
				Callbacks.Pop()?.Callback?.Invoke();
			}

			while(InsteadNextCommandIfPredicateCallbacks.Any())
			{
                (RenSharpCallback, Func<bool>) callbackPredicate = InsteadNextCommandIfPredicateCallbacks.Pop();
				Func<bool> predicate = callbackPredicate.Item2;
				if (predicate?.Invoke() == true)
				{
                    containsInsteadNextCommandCallback = true;
					callbackPredicate.Item1?.Callback?.Invoke();
                }
            }

			return containsInsteadNextCommandCallback;

        }

		private Command ExecuteNext()
		{
            Command command;
            bool skip;

            do
            {
                skip = false;
                bool hasNext = Program.MoveNext();
                if (!hasNext)
                    throw new UnexpectedEndOfProgramException("Неожиданный конец программы.");

                command = Program.Current;
                if (command.Level > Context.Level)
                {
                    skip = true;
                    continue;
                }

                int cycleStart = 0;
                while (command.Level < Context.Level)
                {
                    cycleStart = Context.LevelStack.Pop();
                    if (cycleStart != 0)
                        break;
                }

                if (cycleStart != 0)
                {
                    Program.Goto(cycleStart);
                    skip = true;
                    continue;
                }

				if(command is IRollbackable)
				{
                    Command backwardCommand = (command as IRollbackable)?.Rollback(this);
                    if (backwardCommand != null)
                        Context.RollbackStack.Push(backwardCommand);
                }
                

                command.Execute(this);
                (command as IPushable)?.TryPush(Context);
            } while (Configuration.IsSkip(command) || skip);

			return command;
        }

		public void ClearRollback()
		{
			Context.ClearRollback();
			Context.MessageHistory.Clear();
		}

		public void Goto(string labelName) => Goto(Program.GetLabel(labelName));
        public void Goto(Command command) => Context.Goto(command);

        public Attributes GetCharacterAttributes(string characterName)
        {
			Character character;
            try
			{
				string systemName = Context.GetVariable<string>(characterName);
				bool exists = CharacterRepository.TryGetCharacter(systemName, out character);
				if (exists == false)
					throw new InvalidOperationException();
				if (character == null)
					throw new InvalidOperationException();

				return character.Attributes;
			}
			catch
			{
				throw new InvalidOperationException($"Персонаж с именем '{characterName}' не найден.");
			}
        }

		public object GetVariable(string name)
			=> Context.GetVariable(name);
		public T GetVariable<T>(string name)
			=> Context.GetVariable<T>(name);

		public void SetVariable(string name, object value)
			=> Context.SetVariable(name, value);

		public ReactiveRS<T> CreateReactive<T>(string name, T value)
			=> new ReactiveRS<T>(this, name, value);

		public void Pause() => _isPaused = true;
		public void Resume() => _isPaused = false;

		private bool IsPaused()
		{
			bool isPausedByPredicate = false;
			var nextPausePredicates = new Stack<Func<RenSharpCore, PausePredicateAction>>();

			while (PausePredicates.Any())
			{
				Func<RenSharpCore, PausePredicateAction> pausePredicate = PausePredicates.Pop();
				PausePredicateAction pausePredicateActions = pausePredicate?.Invoke(this)
					?? throw new InvalidOperationException("Pause predicate was null.");

				if(pausePredicateActions.HasFlag(PausePredicateAction.Pause))
					isPausedByPredicate = true;

				if(pausePredicateActions.HasFlag(PausePredicateAction.Unsubscribe) == false)
					nextPausePredicates.Push(pausePredicate);
			}

			PausePredicates = nextPausePredicates;

			return _isPaused || isPausedByPredicate;
		}

		public void AddPausePredicate(Func<RenSharpCore, PausePredicateAction> pausePredicate)
			=> PausePredicates.Push(pausePredicate);

		private void ExecuteInits()
        {
			IEnumerable<Init> inits = Context.Program.Code
				.OfType<Init>()
				.OrderByDescending(x => x.Priority)
				.ThenBy(x => x.Line);

			IEnumerable<Define> defines = Context.Program.Code
				.OfType<Define>();

			foreach(Define define in defines)
			{
				define.ExecuteDefine(this);
			}

			foreach (Init init in inits)
			{
				Command initStart = Program[init.Line + 1];
				Goto(initStart);
				Command command;

				while (true)
				{
					bool hasNext = Program.MoveNext();
					if (!hasNext)
						break;

					command = Program.Current;
					if (command.Level > Context.Level)
						continue;

					int cycleStart = 0;
					while (command.Level < Context.Level)
					{
						cycleStart = Context.LevelStack.Pop();
						if (cycleStart != 0)
							break;
					}
					if (cycleStart != 0)
					{
						Program.Goto(cycleStart);
						continue;
					}

					if (command.Level <= 1)
						break;

					command.Execute(this);
				}
			}
		}
    }
}
