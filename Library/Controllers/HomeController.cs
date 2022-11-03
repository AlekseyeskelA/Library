using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static Library.Models.Models;                                         // Чтобы была видна DataStorage.

namespace Library.Controllers
{
    public class HomeController : Controller
    {
        // Метод 1. Отображение всех книг:
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Filtres = new Book { ISBN = null, Name = null, Author = null, Publisher = null, Year = null, Num = null };  // При первом открытии сайта все фильтры сброшены.
            ViewBag.Books = DataStorage.Books;                              // Передаем все объекты Books в динамическое свойство Books объекта ViewBag.            
            return View();                                                  // Возвращаем представление.
        }


        // Метод 2. Отображение всех книг с применением фильтра:
        [HttpPost]
        public ActionResult Index(Book book)
        {
            if (book.ISBN == null &&                                        // При сброшенном фильтре отображаются все книги.
                book.Name == null &&
                book.Author == null &&
                book.Publisher == null &&
                book.Year == null &&
                book.Num == null)
            {
                ViewBag.Books = DataStorage.Books;
                ViewBag.Filtres = new Book { ISBN = null, Name = null, Author = null, Publisher = null, Year = null, Num = null };
                return View();
            }
            else
            {                                                               // Если фильтры активированы, то отображается только нужные книги.
                List<Book> FilteredBooks = new List<Book>();
                int countB = DataStorage.Books.Count;
                // Фильтр по принципу ИЛИ:
                for (int i = 0; i < countB; i++)
                    if (DataStorage.Books[i].ISBN == book.ISBN ||
                        DataStorage.Books[i].Name == book.Name ||
                        DataStorage.Books[i].Author == book.Author ||
                        DataStorage.Books[i].Publisher == book.Publisher ||
                        DataStorage.Books[i].Year == book.Year ||
                        DataStorage.Books[i].Num == book.Num)

                        // Фильтр по принципу И:
                    //if ((DataStorage.Books[i].ISBN == book.ISBN || book.ISBN == null) &&
                    //(DataStorage.Books[i].Name == book.Name || book.Name == null) &&
                    //(DataStorage.Books[i].Author == book.Author || book.Author == null) &&
                    //(DataStorage.Books[i].Publisher == book.Publisher || book.Publisher == null) &&
                    //(DataStorage.Books[i].Year == book.Year || book.Year == null) &&
                    //(DataStorage.Books[i].Num == book.Num || book.Num == null))
                            FilteredBooks.Add(DataStorage.Books[i]);        // Список книг, соответствующий выбранным фильтрам.

                ViewBag.Books = FilteredBooks;
                ViewBag.Filtres = new Book { ISBN = book.ISBN, Name = book.Name, Author = book.Author, Publisher = book.Publisher, Year = book.Year, Num = book.Num };  // Чтобы параметры фильтром запоминались.

                return View();
            }
        }


        // Метод 3. Добавление книг в хранилище:
        // (Является распространённой техникой, когда идёт запрос пользователя, создавать два метода с одинаковыми названиями, но который различаются типом запроса)
        public ActionResult AddBook()
        {
            return View();
        }


        // Метод 4. Добавление книги в хранилище:
        // (обработка данных, введённых пользователем):
        [HttpPost]                                                          // [HttpPost] - метод вызывается, когда пользователь что-то отправляет на сервер.
        public ActionResult AddBook(Book book)
        {
            DataStorage.Books.Add(book);
            return View();
        }


        // Метод 5. Выдача книг читателям:
        [HttpGet]
        public ActionResult Get(string id)
        {
            int countB = DataStorage.Books.Count;
            for (int i = 0; i < countB; i++)                                // Обходим в цикле все книги...
                if (DataStorage.Books[i].ISBN == int.Parse(id))             // ...и нахоим нужную книгу.
                    if (DataStorage.Books[i].Num == 0)                      // Отказываем, если таких книг больше нет.
                        return Redirect("/Home/Unsuccessful");                  
                    else DataStorage.Books[i].Num--;                        // Если книги есть, то уменьшаем количество экземпляров в библиотеке.

            ViewBag.ISBN = id;
            return View();
        }


        // Метод 5. Выдача книг читателям (запись в личном деле читателя):
        [HttpPost]
        public RedirectResult Get(ReaderTemp rt)                            // Получаем экземпляр вспомогательного класса, соединяющий читателя и взятую им книгу.
        {
            bool bReaderFined = false;
            bool bBookFined = false;
            int countBOH = DataStorage.BooksOnHands.Count;
            for (int i = 0; i < countBOH; i++)
                if (DataStorage.BooksOnHands[i].Name == rt.Name)            // Ищем данного читателя в списке библиотеки.
                {
                    bReaderFined = true;
                    int countTB = DataStorage.BooksOnHands[i].TakenBooks.Count;
                    for (int j = 0; j < countTB; j++)                        
                        if (DataStorage.BooksOnHands[i].TakenBooks[j].ISBN == rt.ISBN)  // Если у читателя уже есть такая книга,...
                        {
                            bBookFined = true;
                            DataStorage.BooksOnHands[i].TakenBooks[j].Num++;// ...то увеличиваем количество взятых им экземпляров.
                            break;
                        }
                    if (bBookFined == false)                                // Если это первая книга у читателя с тиким ISBN, то добавляем её в формуляр и пишем количество: 1.
                    {
                        DataStorage.BooksOnHands[i].TakenBooks.Add(new Reader.STakenBooks { ISBN = rt.ISBN, Num = 1 });
                        break;
                    }
                }
            if (bReaderFined == false)                                      // Если читатель такой не найден, значит это новый клиент библиотеки.
            {
                DataStorage.BooksOnHands.Add(new Reader { Name = rt.Name, TakenBooks = new List<Reader.STakenBooks> { new Reader.STakenBooks {ISBN = rt.ISBN, Num = 1 } } });                
            }
            return Redirect("/Home/Successful");                            // Информируем о положительном результате.
        }


        // Метод 6. Сообщение об успешной операции:
        public ActionResult Successful()
        {
            return View();
        }


        // Метод 7. Отказ в выдаче книги:        
        public ActionResult Unsuccessful()
        {
            return View();
        }


        // Метод 8. Список читалелей и книг на руках:

        public ActionResult UsingBooks()
        {
            ViewBag.BooksOnHands = DataStorage.BooksOnHands;
            return View();
        }


        // Метод 9. Изменение данных книги:
        [HttpGet]
        public ActionResult ChangeInfo(Book b)
        {
            ViewBag.Book = b;
            return View();
        }


        // Метод 10. Изменение данных книги (передача информации на сервер):
        [HttpPost]
        public ActionResult ChangeInfoPOST(Book b)                          // Можно поменять название метода POST (при совпадении аргументов), если на страничке ChangeInfo написать <form method="post" action="ChangeInfoPOST">
        {
            int countB = DataStorage.Books.Count;
            for (int i = 0; i < countB; i++)
            {
                if (DataStorage.Books[i].ISBN == b.ISBN)
                {
                    DataStorage.Books[i].Author = b.Author;
                    DataStorage.Books[i].Name = b.Name;
                    DataStorage.Books[i].Num = b.Num;
                    DataStorage.Books[i].Publisher = b.Publisher;
                    DataStorage.Books[i].Year = b.Year;
                    break;
                }
            }
            return Redirect("/Home/Successful");
        }


        // Метод 11. Удаление книги:
        [HttpGet]
        public ActionResult Delete(Book b)
        {
            ViewBag.Book = b;
            return View();
        }


        // Метод 12. Удаление книги:
        [HttpPost]
        public RedirectResult Delete(int ISBN)
        {
            for (int i = 0; i < DataStorage.Books.Count; i++)
                if (DataStorage.Books[i].ISBN == ISBN)
                {
                    DataStorage.Books.RemoveAt(i);
                    break;
                }
            return Redirect("/Home/Successful");
        }


        // Метод 13. Возврат книги в библиотеку:
        public ActionResult Return(ReaderTemp rt)                           // Получаем данные о читателе и возвращаемой им книге.
        {
            // Изменения в библиотеке:
            int countB = DataStorage.Books.Count;
            bool bBookFined = false;
            for (int i = 0; i < countB; i++)
            {
                if (DataStorage.Books[i].ISBN != rt.ISBN) continue;
                else
                {
                    DataStorage.Books[i].Num++;                             // Находим нужную книгу в библиотеке и прибавляем один экземпляр.
                    bBookFined = true;
                    break;
                }                
            }
            if (bBookFined == false)
                DataStorage.Books.Add(new Book { ISBN = rt.ISBN, Num = 1 });// Если такая книга не дайдена (например, после удаления), то создаётся новая позиция.

            // Изменения в личном деле читателя:
            int countBOH = DataStorage.BooksOnHands.Count;
            for (int i = 0; i < countBOH; i++)                              
            {
                if (DataStorage.BooksOnHands[i].Name == rt.Name)            // Находим нужного читателя.
                {
                    int countTB = DataStorage.BooksOnHands[i].TakenBooks.Count;
                    for (int j = 0; j < countTB; j++)
                        if (DataStorage.BooksOnHands[i].TakenBooks[j].ISBN == rt.ISBN && DataStorage.BooksOnHands[i].TakenBooks[j].Num > 1)
                        {
                            DataStorage.BooksOnHands[i].TakenBooks[j].Num--;// Уменьшаем количество книг, если у читателя на руках несколько экземпляров.
                            return Redirect("/Home/Successful");
                        }                            
                        else if (DataStorage.BooksOnHands[i].TakenBooks[j].ISBN == rt.ISBN && DataStorage.BooksOnHands[i].TakenBooks[j].Num == 1)
                        {
                            DataStorage.BooksOnHands[i].TakenBooks.RemoveAt(j);// Если книга была одна, то убираем её из списка.
                            if (DataStorage.BooksOnHands[i].TakenBooks.Count == 0)
                            {
                                DataStorage.BooksOnHands.RemoveAt(i);       // Если читатель сдал все книги, то удаляем его из базы данных.
                                return Redirect("/Home/Successful");        // Чтобы выйти из цикла без ошибки после удаления читателя.
                            }
                            return Redirect("/Home/Successful");            // Чтобы выйти из цикла без ошибки после удаления книги.
                        }
                }
            }
            return Redirect("/Home/Successful");
        }
    }
}
