namespace RenSharp.Models
{
	/// <summary>
	/// Rollbackable commands способны создавать обратную команду
	/// обратная команда отменяет является противоположность обычной команды
	/// и отменяет ее дейсвтия.Такие команды как message в обратную сторону эквиваленты.
	/// </summary>
	public interface IRollbackable
	{
		/* 
		 
		 Пример обратной программы:

		    [Обычная программа]    [Обратная программа]
		                         
		1|  x = 5                | del x
		2|  x = x + 10           | x = 5
		3|  "Hello, world! {x}"  | "Hello, world! {x}"
		4|  y = 42               | del y
		5|  x = y + 8            | x = 15

		*/
		Command Rollback();
	}
}
