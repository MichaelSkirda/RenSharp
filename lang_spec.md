# RenSharp - Language specification

## Core commands

### say statement

**TL;TR**

Выводит имя и фразу персонажа на экран.

**Как работает на самом деле**

Формирует объект MessegeResult и отдает его IWriter если он не равен null. Реализация IWriter передается при создании объекта RenSharp внутри Configuration. Стандартная реализация ConsoleWriter выводит MessageResult в консоль, в формате "{name}: {text}", например: "Alice: Oh, hi Bob!".

Стандартная Unity реалзиация отобразит имя и текст в специальных для них блоках.

Пока не поддерживает свойства и проблемы с отображением текста. В будущих версиях это будет исправлено.

```
define a = Character("Alice")
define bb = Character("Bob")
define st = Character("Storyteller")

label start

	st "Alice and Bob met in mall."
	a "Oh, hi Bob!"
	bb "Hi, Alice!"

```

### python statement
Вы можете писать python код внутри блока python.

```py
start:
    python:
        def Sum(a, b):
            return a + b
        foo = "Hello, "
        greetings = Sum(foo, " bob")
        
    alice "{greetings}" # Hello, bob
    ...
```

Также есть возможность использовать однострочный python statement:
```py
$ coolestLanguage = "RenSharp"
alice "It is a {coolestLanguage} code"
bob "Cool."
```

You also can combine python and init statements:

```
init 100 python:
	print("My priority is 100. I'm second.");

init 200 python:
	print("My priority is 200. I'm first.");

label start:
	"Game start"
	"Game end"
	jump start
```

### Label/Jump/Call/Return statements

Команды Jump и Call переносят поток выполнения программы на указанный Label. 

```
label start:
    a "Hello, Bob!"
    a "How are you?"
    jump alice_sad

label bob_try_say:
    b "Hi Alice."
    b "Why are you silent?"
    b "Do you hear me Alice?"

label alice_sad:
    a "Why you ignore me? :("
```

Также можно сделать Jump/Call на start, но это не перезапустит программу, все переменные и Rollback останется прежним.

```
init python:
    x = 1;
label start
    "Hello, i saw you {x} times!"
    x++
    jump start
```

#### Call vs Jump?

Хоть Call и Jump похожи на первый взгляд между ними есть значительная разница.

Jump - только переносит поток выполнения программы на Label.

Call - переносит поток выполнения программы на Label в стеке вызовов. Если внутри Label есть Return то после этой команды программа продолжит выполнение с места Call.

Если стек вызовов пустой Return завершит программу или вернет в главное меню.

Call:
```
start:
    "Start"
    call return_test
    "End"

label return_test:
    a "What is return?"
    a "Hmm... Let's try!"
    return
    a "Now nobody hears me :("

# Nobody: Start
# Alice: What is return?
# Alice: Hmm.. Let's try!
# Nobody: End

```

Jump:
```
start:
    jump return_test
    "End of program!"

label return_test:
    a "What is return?"
    a "Hmm... Let's try!"
    return
    a "Now nobody hears me :("

# Nobody: Start
# Alice: What is return?
# Alice: Hmm.. Let's try!
# Program will end or return to main menu.

```

## Unity commands