//  CSharp/Seminar 4


System.Console.Write("Введите номер задачи: ");

if (int.TryParse(Console.ReadLine(), out int select))
{

//////// Задача 1 ////////
    
    if(select == 1)
    {
        while(true)
        {
            int? sum = 0;
            string input;

            Console.Write("Введите целое число или 'q' для выхода: ");
            input = Console.ReadLine() ?? string.Empty;

            if (input.Equals("q", StringComparison.OrdinalIgnoreCase))
            {
                System.Console.WriteLine("Программа завершена.");
                break;
            }

            foreach (char digit in input)
            {
                if (char.IsDigit(digit) && sum != null)
                    sum += digit - '0';
                else if(digit != '-')
                    sum = null;
            }

            if (sum % 2 == 0 && sum != 0)
            {
                System.Console.WriteLine("Программа завершена.");
                break;
            } 
        }
    }

//////// Задача 2 ////////
    
    else if(select == 2)
    {
        var arrayTask2 = new int[4];

        randomArray(arrayTask2);
     
        System.Console.WriteLine($"[ {string.Join(" ", arrayTask2)} ] => {evenCount(arrayTask2)}");
    }

//////// Задача 3 ////////
    
    else if(select == 3)
    {
        int[] arrayTask3 = [1, 3, 5, 6, 7, 8];
        Console.Write($"[{string.Join(", ", arrayTask3)}]");
        
        Array.Reverse(arrayTask3);
        System.Console.WriteLine($" => [{string.Join(", ", arrayTask3)}]"); 
    }
    else
        System.Console.WriteLine("Введите число от 1 до 3.");
}
else
    System.Console.WriteLine("Ошибка ввода.");


//////// Задача 2 - Функции ////////

static void randomArray(int[] array)
{
    Random random = new();
    for(int i = 0; i < array.Length; i++)
            array[i] = random.Next(100, 1000);

}

static int evenCount(int[] array)
{
    int count = 0;

    foreach(int num in array)
        if (num % 2 == 0)
            count++;  
        
    return count;
}

////////////////////////////////////



