using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models
{
    public class Models
    {
        // Класс Книга: 
        public class Book
        {
            public int? ISBN { get; set; }                                  // Nullable-тип для корректной работы фильтра книг на основной странице.
            public string Name { get; set; }
            public string Author { get; set; }
            public string Publisher { get; set; }
            public int? Year { get; set; }                                  // Nullable-тип для корректной работы фильтра книг на основной странице.
            public int? Num { get; set; }                                   // Nullable-тип для корректной работы фильтра книг на основной странице.
        }

        // Класс Читатель:
        public class Reader
        {
            public string Name { get; set; }                                // Имя читателя.
            public class STakenBooks                                        // У каждого читателя на руках может несколько одинаковых книг.
            {
                public int ISBN { get; set; }
                public int Num { get; set; }
            }
            public List<STakenBooks> TakenBooks { get; set; }               // У каждого читателя на руках может быть несколько разных книг.
        }

        // Вспомогательный класс Читатель
        //(используется для операций выдачи и возврата книг, как структура более простая, чем класс Reader, хранящий полную информацию о читателе):
        public class ReaderTemp
        {
            public int ISBN { get; set; }
            public string Name { get; set; }
            public int Num { get; set; }
        }

        // Класс базы данных. (Static - для простоты доступа):
        public static class DataStorage                         
        {
            // Книги в библиотеке:
            public static List<Book> Books = new List<Book>
            {
                new Book() {Author = "Пушкин А.С.", ISBN = 113138848, Name = "Сборник стихов", Publisher = "Искусство", Year = 1992, Num = 3},
                new Book() {Author = "Толстой Л.Н.", ISBN = 423425334, Name = "Война и мир (том 1)", Publisher = "Художественная литература", Year = 1980, Num = 5},
                new Book() {Author = "Толстой Л.Н.", ISBN = 749145514, Name = "Война и мир (том 2)", Publisher = "Художественная литература", Year = 1980, Num = 7},
                new Book() {Author = "Гоголь Н.В.", ISBN = 412432131, Name = "Ревизор",Publisher = "Просвещение", Year = 1963, Num = 1}
            };

            // Список читателей и взятые ими книги:
            public static List<Reader> BooksOnHands = new List<Reader>
            {
                new Reader()
                {
                    Name = "Алексей",
                    TakenBooks = new List<Reader.STakenBooks>
                    {
                        new Reader.STakenBooks {ISBN=113138848, Num=1 },
                        new Reader.STakenBooks {ISBN=749145514, Num=2 }
                    }
                },

                new Reader()
                {
                    Name = "Николай",
                    TakenBooks = new List<Reader.STakenBooks>
                    {
                        new Reader.STakenBooks {ISBN=412432131, Num=3 },
                        new Reader.STakenBooks {ISBN=423425334, Num=1 }
                    }
                }                
            };
        }            
    }
}
