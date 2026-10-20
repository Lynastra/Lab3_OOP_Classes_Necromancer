using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OOP_Lab3_Classes
{
    // Родительский базовый класс "миньон"
    // Абстрактный базовый класс: объект "просто миньон" создать нельзя.
    public abstract class Minion
    {
        // Статическое поле: сколько миньонов поднято за всё время.
        // Читать можно откуда угодно, менять - только внутри класса.
        public static int TotalMinions { get; private set; } = 0;

        private int _health;

        public string MinionName { get; set; }

        // Чтение и запись, с проверкой в set: здоровье не уходит в минус
        public int Health
        {
            get => _health;
            set => _health = Math.Max(0, value);
        }

        // protected: вызвать можно только из наследников
        protected Minion(string name, int health)
        {
            TotalMinions++;
            MinionName = name;
            Health = health;
            Console.WriteLine($"[Minion] конструктор: {GetType().Name} '{MinionName}' (HP: {Health}) восстал из мёртвых!");
        }

        // Виртуальный метод: наследники могут его переопределить
        public virtual void Attack()
        {
            Console.WriteLine($"  -> Миньон '{MinionName}' наносит слабый удар.");
        }

        ~Minion()
        {
            Console.WriteLine($"[Minion] деструктор: {GetType().Name} '{MinionName}' рассыпался в прах.");
        }
    }

    public class Skeleton : Minion
    {
        public Skeleton(string name, int health) : base(name, health)
        {
            Console.WriteLine($"[Skeleton] конструктор: '{MinionName}'");
        }

        public override void Attack()
        {
            Console.WriteLine($"  -> Скелет '{MinionName}' бьёт ржавым мечом!");
        }

        ~Skeleton() => Console.WriteLine($"[Skeleton] деструктор: кости '{MinionName}' рассыпались...");
    }

    public class Spirit : Minion
    {
        public Spirit(string name, int health) : base(name, health)
        {
            Console.WriteLine($"[Spirit] конструктор: '{MinionName}'");
        }

        public override void Attack()
        {
            Console.WriteLine($"  -> Дух '{MinionName}' выпускает ледяную стрелу!");
        }

        ~Spirit() => Console.WriteLine($"[Spirit] деструктор: дух '{MinionName}' растворился в тумане...");
    }

    public class BloodGolem : Minion
    {
        public BloodGolem(string name, int health) : base(name, health)
        {
            Console.WriteLine($"[BloodGolem] конструктор: '{MinionName}'");
        }

        public override void Attack()
        {
            Console.WriteLine($"  -> Кровавый голем '{MinionName}' сокрушает врага кулаком и лечится на 10 HP!");
            Health += 10;
        }

        ~BloodGolem() => Console.WriteLine($"[BloodGolem] деструктор: БУМ! Голем '{MinionName}' взорвался!");
    }

    // Родительский базовый класс персонаж
    public class Character
    {
        // Статическое поле: счётчик персонажей
        public static int TotalCharacters { get; private set; } = 0;

        private int _health;

        // Поле, которое потом будет скрыто в наследнике (пример скрытия поля)
        public string Title = "Герой";

        // Свойство только для чтения (значение задаётся в конструкторе)
        public string Name { get; }

        // Свойство для чтения и записи, с проверкой в set
        public int Health
        {
            get => _health;
            set => _health = Math.Max(0, value);
        }

        // Конструктор по умолчанию: передаёт управление другому конструктору этого же класса
        public Character() : this("Безымянный герой", 100)
        {
            Console.WriteLine("[Character] конструктор по умолчанию");
        }

        // Конструктор с аргументами
        public Character(string name, int health)
        {
            TotalCharacters++;
            Name = name;
            Health = health;
            Console.WriteLine($"[Character] конструктор с аргументами: '{Name}' (HP: {Health})");
        }

        // Не virtual: в наследнике его можно только скрыть через new
        public void Attack()
        {
            Console.WriteLine($"{Name} наносит базовый удар.");
        }

        ~Character()
        {
            Console.WriteLine($"[Character] деструктор: '{Name}'");
        }
    }

    // класс-наследник некромант
    public class Necromancer : Character
    {
        // Поле-экземпляр параметризованного класса
        private readonly List<Minion> _minions = new List<Minion>();
        private string? _demonName;

        // Скрываем поле базового класса
        public new string Title = "Некромант";

        // Свойство только для записи
        public string BindDemon
        {
            set
            {
                _demonName = value;
                Console.WriteLine($"{Name} вписывает в гримуар имя демона: '{value}'. Демон связан клятвой!");
            }
        }

        // Количество миньонов: свойство только для чтения (вычисляемое)
        public int MinionCount => _minions.Count;

        // Конструктор по умолчанию: делегирует своему конструктору через this(...)
        public Necromancer() : this("Безымянный некромант", 100)
        {
            Console.WriteLine("[Necromancer] конструктор по умолчанию");
        }

        // Конструктор с аргументами: делегирует конструктору родителя через base(...)
        public Necromancer(string name, int health) : base(name, health)
        {
            Console.WriteLine($"[Necromancer] конструктор с аргументами: '{Name}'");
        }

        // Принимает любого наследника Minion
        public void RaiseMinion(Minion newMinion)
        {
            ArgumentNullException.ThrowIfNull(newMinion);
            _minions.Add(newMinion);
        }

        public void CommandMinions()
        {
            if (_minions.Count == 0)
            {
                Console.WriteLine($"{Name} пытается приказать, но армия мёртвых пуста.");
                return;
            }

            Console.WriteLine($"{Name} отдаёт приказ 'Атакуйте!'");
            foreach (Minion minion in _minions)
            {
                // Полиморфизм: для каждого вызовется свой Attack()
                minion.Attack();
            }
        }

        // Использование записанного через BindDemon имени
        public void SummonDemon()
        {
            if (_demonName == null)
                Console.WriteLine($"{Name} не знает имени демона, призывать некого.");
            else
                Console.WriteLine($"{Name} призывает демона '{_demonName}'!");
        }

        // Скрытие метода базового класса (new)
        public new void Attack()
        {
            Console.WriteLine($"{Name} (Некромант) выпускает стрелу костяной магии!");
        }

        // Обращение к скрытому методу базового класса
        public void AttackHiddenBase()
        {
            base.Attack();
        }

        // Обращение к скрытому полю: свое поле и поле родителя
        public void ShowTitles()
        {
            Console.WriteLine($"Title (свой): {Title} / base.Title (родителя): {base.Title}");
        }

        public void ClearArmy()
        {
            Console.WriteLine("Армия мёртвых отпущена...");
            _minions.Clear(); // стираем ссылки, миньоны становятся мусором
        }

        ~Necromancer()
        {
            Console.WriteLine($"[Necromancer] деструктор: '{Name}' ушёл в небытие.");
        }
    }

    // демострация работы
    internal class Program
    {
        // NoInlining + отдельный метод: после возврата из него локальных ссылок
        // не остаётся, и GC.Collect() в Main реально соберёт объекты (в т.ч. в Debug).
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void RunDemo()
        {
            Console.WriteLine("=== 1. Конструкторы: по умолчанию и с аргументами ===");
            Character hero = new Character();
            Console.WriteLine();
            Necromancer anon = new Necromancer();
            Console.WriteLine();
            Necromancer necro = new Necromancer("Мираэль", 100);
            Console.WriteLine($"\nПерсонажей создано (static): {Character.TotalCharacters}");

            Console.WriteLine("\n=== 2. Свойства разных видов ===");
            Console.WriteLine($"Имя (только чтение): {necro.Name}");
            necro.Health = 80;                       // чтение и запись
            Console.WriteLine($"Здоровье: {necro.Health}");
            necro.Health = -50;                      // set не даст уйти в минус
            Console.WriteLine($"Здоровье после -50: {necro.Health}");
            necro.BindDemon = "Баал";                // только запись
            necro.SummonDemon();
            // necro.Name = "Другой";                // ошибка: нет set
            // Console.WriteLine(necro.BindDemon);   // ошибка: нет get

            Console.WriteLine("\n=== 3. Скрытие членов базового класса (new) ===");
            necro.Attack();                          // версия Necromancer
            necro.AttackHiddenBase();                // версия Character через base.
            necro.ShowTitles();                      // оба поля Title

            Console.WriteLine("\n=== 4. Совместимость типов ===");
            Character baseRef = necro;               // восходящее преобразование: можно неявно
            baseRef.Attack();                        // Attack скрыт (new), а не переопределён -> Character.Attack
            Console.WriteLine($"Title через ссылку Character: {baseRef.Title}");
            // baseRef.CommandMinions();             // ошибка: доступ определяется типом ссылки
            // necro = baseRef;                      // ошибка: нужно явное приведение

            if (baseRef is Necromancer castedNecro)  // нисходящее: безопасно через is / as
            {
                Console.WriteLine("Нисходящее преобразование прошло успешно:");
                castedNecro.Attack();
            }

            Minion m = new Skeleton("Кость", 50);    // Skeleton -> Minion
            Spirit? notSpirit = m as Spirit;         // скелет не дух -> null
            Console.WriteLine($"m as Spirit == null? {notSpirit == null}");
            // Minion bad = new Minion("x", 1);      // ошибка: класс абстрактный
            // Skeleton s2 = (Skeleton)(Minion)new Spirit("Тень", 1);  // InvalidCastException при запуске

            Console.WriteLine("\n=== 5. Полиморфизм и List<Minion> ===");
            necro.RaiseMinion(m);
            necro.RaiseMinion(new Spirit("Тень", 30));
            necro.RaiseMinion(new BloodGolem("Здоровяк", 200));
            Console.WriteLine($"Миньонов в армии: {necro.MinionCount}, поднято всего (static): {Minion.TotalMinions}");
            necro.CommandMinions();

            Console.WriteLine("\n=== 6. Подготовка к деструкторам ===");
            necro.ClearArmy();
            // Все ссылки (hero, anon, necro, baseRef, m...) исчезнут при выходе из метода
        }

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            RunDemo();

            Console.WriteLine("\n=== 7. Сборка мусора: вызов деструкторов ===");
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("\nКонец программы.");
            Console.ReadKey();
        }
    }
}