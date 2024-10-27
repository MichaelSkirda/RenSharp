using RenSharp.Core;

namespace Assets.Scripts.RenSharp.Core
{
    public class ReactiveRS<T>
    {
        private RenSharpCore _renSharpCore;
        private readonly string _name;

        public T Value
        {
            get
            {
                return _renSharpCore.GetVariable<T>(_name);
            }
            set
            {
                _renSharpCore.SetVariable(_name, value);
            }
        }

        public ReactiveRS(RenSharpCore renSharpCore, string name)
        {
            _renSharpCore = renSharpCore;
            _name = name;
        }

        public ReactiveRS(RenSharpCore renSharpCore, string name, T value)
        {
            _renSharpCore = renSharpCore;
            _name = name;
            Value = value;
        }

        public static implicit operator T(ReactiveRS<T> reactive) => reactive.Value;
        public override string ToString() => Value.ToString();
    }

    public class ReactiveRS : ReactiveRS<dynamic>
    {
        public ReactiveRS(RenSharpCore renSharpCore, string name) : base(renSharpCore, name) { }
        public ReactiveRS(RenSharpCore renSharpCore, string name, dynamic value) : base(renSharpCore, name)
        {
            Value = value;
        }
    }

}
