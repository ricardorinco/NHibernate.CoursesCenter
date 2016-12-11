using ConsoleTables.Core;
using Persistence.Config;
using Persistence.DAO;
using Persistence.Helper;
using Persistence.POCO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ViewConsole
{
    class Program
    {
        private static string option = string.Empty;
        private static SGDB connectedDataBase = SGDB.SQLServer;

        [STAThread]
        static void Main(string[] args)
        {
            Console.SetWindowSize(168, 36);
            do
            {
                Menu();
                option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        Console.WriteLine(Environment.NewLine);
                        Reader();
                        break;
                    case "2":
                        Console.WriteLine(Environment.NewLine);
                        RequestGenerate();
                        break;
                    case "3":
                        Console.WriteLine(Environment.NewLine);
                        SelectAllInstructors();
                        break;
                    case "4":
                        Console.WriteLine(Environment.NewLine);
                        SelectAllCourses();
                        break;
                    case "5":
                        Console.WriteLine(Environment.NewLine);
                        SelectAllStudents();
                        break;
                    case "6":
                        Console.WriteLine(Environment.NewLine);
                        SelectAllRequests();
                        break;
                    case "7":
                        Console.WriteLine(Environment.NewLine);
                        SelectAllRequestDetails();
                        break;
                    case "8":
                        Console.WriteLine(Environment.NewLine);
                        SelectTotalCourseMinisteredByInstructor();
                        break;
                    case "9":
                        Console.WriteLine(Environment.NewLine);
                        SelectTotalRequestCoursePriceByCourseType();
                        break;
                    case "10":
                        Console.WriteLine(Environment.NewLine);
                        SelectTotalRequestCoursePriceByCourse();
                        break;
                    case "11":
                        Console.WriteLine(Environment.NewLine);
                        SelectTotalRequestCoursePriceByStudent();
                        break;
                    case "12":
                        Console.WriteLine(Environment.NewLine);
                        SelectCourseBestSeller();
                        break;
                    case "13":
                        Console.WriteLine(Environment.NewLine);
                        SelectStudentsRegisterInCourses();
                        break;
                    case "14":
                        Console.WriteLine(Environment.NewLine);
                        SelectStudentsWithoutRequests();
                        break;
                    case "15":
                        Console.WriteLine(Environment.NewLine);
                        AlterProviderStudentsEmail();
                        break;
                    case "16":
                        Console.WriteLine(Environment.NewLine);
                        DeleteRequestsWithoutCourses();
                        break;
                    case "99":
                        Console.WriteLine(Environment.NewLine);
                        AlterConnectedSGDB();
                        break;
                    case "0":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(Environment.NewLine);
                        Console.WriteLine("{0} não é uma opção válida.", option);
                        Console.ResetColor();
                        Console.WriteLine(Environment.NewLine);
                        break;
                }

            } while (option != "0");
        }
        private static void Menu()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("┌───────────────────┤ MENU ├─────────────────┐");
            Console.WriteLine("│ Inserts                                    │");
            Console.WriteLine("│ 1. Importar Cadastro de Alunos (*.csv)     │");
            Console.WriteLine("│ 2. Gerar Pedidos Automáticos               │");
            Console.WriteLine("│                                            │");
            Console.WriteLine("│ Selects                                    │");
            Console.WriteLine("│ 3. Instrutores                             │");
            Console.WriteLine("│ 4. Cursos                                  │");
            Console.WriteLine("│ 5. Alunos                                  │");
            Console.WriteLine("│ 6. Pedidos                                 │");
            Console.WriteLine("│ 7. Detalhes dos Pedidos                    │");
            Console.WriteLine("│ 8. Total de Cursos por Instrutor           │");
            Console.WriteLine("│ 9. Total dos Pedidos por Tipo de Curso     │");
            Console.WriteLine("│ 10. Total dos Pedidos por Curso            │");
            Console.WriteLine("│ 11. Total dos Pedidos por Aluno            │");
            Console.WriteLine("│ 12. Curso mais Pedido                      │");
            Console.WriteLine("│ 13. Alunos Matrículados nos Cursos         │");
            Console.WriteLine("│ 14. Alunos que Não Possuem Pedidos         │");
            Console.WriteLine("│                                            │");
            Console.WriteLine("│ Update                                     │");
            Console.WriteLine("│ 15. Alterar E-mail dos Alunos              │");
            Console.WriteLine("│                                            │");
            Console.WriteLine("│ Delete                                     │");
            Console.WriteLine("│ 16. Excluir Pedidos sem Cursos             │");
            Console.WriteLine("│                                            │");
            Console.WriteLine("│ Database                                   │");
            Console.WriteLine("│ 99. Alterar SGDB                           │");
            Console.WriteLine("│                                            │");
            Console.WriteLine("│ 0. Sair                                    │");
            Console.WriteLine("├────────────────────────────────────────────┤");
            Console.WriteLine("│ Escolha uma opção e pressione Enter        │");
            Console.WriteLine("└────────────────────────────────────────────┘");
            Console.Write("Opção: ");
            Console.ResetColor();
        }
        private static void AlterConnectedSGDB()
        {
            if (connectedDataBase == SGDB.SQLServer)
            {
                connectedDataBase = SGDB.MySQL;
            }
            else if (connectedDataBase == SGDB.MySQL)
            {
                connectedDataBase = SGDB.SQLServer;
            }

            Console.WriteLine("O SGBD foi alterado para: {0}.", connectedDataBase.ToString());
            Console.WriteLine(Environment.NewLine);
        }

        private static void Reader()
        {
            try
            {
                Console.WriteLine("Carregando tela de seleção de arquivo.");
                string fileName = string.Empty;

                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Title = "Selecione o Arquivo para Importação";
                fileDialog.Filter = "Comma-separated (*.csv)|*.csv";
                fileDialog.CheckFileExists = true;

                DialogResult result = fileDialog.ShowDialog();
                fileName = fileDialog.FileName;
                if (result == DialogResult.OK)
                {
                    Console.WriteLine("Arquivo carregado: {0}.", fileName);
                    Console.WriteLine("Iniciando a leitura do arquivo.");

                    var lines = File.ReadAllLines(fileName)
                        .Select(a => a.Split(';'))
                        .ToList();

                    if (lines.Count() > 0)
                    {
                        Console.WriteLine("O arquivo possui {0} registros.", lines.Count());

                        StudentDAO studentDAO = new StudentDAO(connectedDataBase);
                        int newStudents = 0;
                        int repeatedStudents = 0;
                        List<Student> students = new List<Student>();
                        foreach (var line in lines)
                        {
                            var student = new Student
                            {
                                Identification = line[0].ToString(),
                                BirthDate = DateTime.Parse(line[1].ToString()),
                                Email = line[2].ToString(),
                                Registration = Int32.Parse(line[3].ToString())
                            };

                            var verifyStudent = studentDAO.GetByIdentification(student.Identification);
                            if (verifyStudent == null)
                            {
                                students.Add(student);
                                newStudents++;
                            }
                            else
                            {
                                repeatedStudents++;
                            }
                        }

                        var stopwatch = new Stopwatch();
                        stopwatch.Start();
                        if (students.Count() > 0)
                            studentDAO.Save(students);

                        stopwatch.Stop();

                        if (newStudents > 0)
                        {
                            Console.WriteLine("Foram inseridos {0} novos registros.", newStudents);
                            Console.WriteLine(Environment.NewLine);
                        }

                        if (repeatedStudents > 0)
                        {
                            Console.WriteLine("{0} registros foram ignorados pois já existem na base de dados.", repeatedStudents);
                            Console.WriteLine(Environment.NewLine);
                        }

                        Console.WriteLine("Desempenho da operação: {0}.", stopwatch.Elapsed);
                        Console.WriteLine("SGBD: {0}.", connectedDataBase.ToString());
                        Console.WriteLine(Environment.NewLine);
                    }
                    else
                    {
                        Console.WriteLine("O arquivo selecionado não possui registros.");
                        Console.WriteLine(Environment.NewLine);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Processo de importação de arquivo cancelado.");
                    Console.WriteLine(Environment.NewLine);
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                System.Console.WriteLine("Erro do sistema: {0}.", ex.Message);
                System.Console.ResetColor();
                System.Console.WriteLine(System.Environment.NewLine);
            }
        }
        private static void RequestGenerate()
        {
            try
            {
                RequestDetailDAO requestDetailDAO = new RequestDetailDAO(connectedDataBase);
                RequestDAO requestDAO = new RequestDAO(connectedDataBase);
                StudentDAO studentDAO = new StudentDAO(connectedDataBase);
                CourseDAO courseDAO = new CourseDAO(connectedDataBase);

                Console.Write("Informe a quantidade de pedidos: ");
                option = Console.ReadLine();

                int optionValue;
                if (Int32.TryParse(option, out optionValue))
                {
                    Console.WriteLine("\nAguarde enquanto o sistema gera os pedidos.");

                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    for (int i = optionValue; i > 0; i--)
                    {
                        Request request = new Request
                        {
                            RequestDateTime = Aleatory.GetDateTime(2015, 2016),
                            Student = studentDAO.GetRandom()
                        };
                        requestDAO.Save(request);

                        int sort = Aleatory.GetInteger(1, 4);
                        while (sort > 0)
                        {
                            RequestDetail requestDetail = new RequestDetail
                            {
                                Request = new Request
                                {
                                    Id = requestDAO.GetLastId()
                                },

                                Course = new Course
                                {
                                    Id = courseDAO.GetRandom().Id
                                }
                            };
                            requestDetailDAO.Save(requestDetail);

                            sort--;
                        }
                    }
                    stopwatch.Stop();

                    Console.WriteLine("Pedidos gerados com sucesso.");
                    Console.WriteLine("\nDesempenho da operação: {0:hh\\:mm\\:ss}.", stopwatch.Elapsed);
                    Console.WriteLine("SGBD: {0}.", connectedDataBase.ToString());
                    Console.WriteLine(Environment.NewLine);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("{0} não foi reconhecido como um número válido.", option);
                    Console.ResetColor();
                    Console.WriteLine(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                System.Console.WriteLine("Erro do sistema: {0}.", ex.Message);
                System.Console.ResetColor();
                System.Console.WriteLine(System.Environment.NewLine);
            }
        }

        private static void SelectAllInstructors()
        {
            try
            {
                InstructorDAO instructorDAO = new InstructorDAO(connectedDataBase);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                ICollection<Instructor> instructors = instructorDAO.GetList();
                stopwatch.Stop();

                if (instructors.Count() > 0)
                {
                    Console.WriteLine("Listagem dos instrutores cadastrados no sistema\n");
                    var table = new ConsoleTable("Id", "Identificação");
                    foreach (var instructor in instructors)
                    {
                        table.AddRow(
                            instructor.Id,
                            instructor.Identification
                        );
                    }
                    table.Write(Format.MarkDown);
                    Console.WriteLine("{0} registros encontrados.", instructors.Count());
                    Console.WriteLine("\nDesempenho da operação: {0:hh\\:mm\\:ss}.", stopwatch.Elapsed);
                    Console.WriteLine("SGBD: {0}.", connectedDataBase.ToString());
                    Console.WriteLine(Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Nenhum resultado encontrado.");
                    Console.WriteLine(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                System.Console.WriteLine("Erro do sistema: {0}.", ex.Message);
                System.Console.ResetColor();
                System.Console.WriteLine(System.Environment.NewLine);
            }
        }
        private static void SelectAllCourses()
        {
            try
            {
                CourseDAO courseDAO = new CourseDAO(connectedDataBase);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                ICollection<Course> courses = courseDAO.GetList();
                stopwatch.Stop();

                if (courses.Count() > 0)
                {
                    Console.WriteLine("Listagem dos cursos cadastrados no sistema\n");
                    var table = new ConsoleTable("Id", "Identificação", "Preço", "Tipo do Curso", "Instrutor");
                    foreach (var course in courses)
                    {
                        table.AddRow(
                            course.Id.ToString(),
                            course.Identification,
                            course.Price.ToString("C"),
                            course.CourseType.Identification,
                            course.Instructor.Identification
                        );
                    }
                    table.Write(Format.MarkDown);
                    Console.WriteLine("{0} registros encontrados.", courses.Count());
                    Console.WriteLine("\nDesempenho da operação: {0:hh\\:mm\\:ss}.", stopwatch.Elapsed);
                    Console.WriteLine("SGBD: {0}.", connectedDataBase.ToString());
                    Console.WriteLine(Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Nenhum resultado encontrado.");
                    Console.WriteLine(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                System.Console.WriteLine("Erro do sistema: {0}.", ex.Message);
                System.Console.ResetColor();
                System.Console.WriteLine(System.Environment.NewLine);
            }
        }
        private static void SelectAllStudents()
        {
            try
            {
                StudentDAO studentDAO = new StudentDAO(connectedDataBase);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                ICollection<Student> students = studentDAO.GetList();
                stopwatch.Stop();

                if (students.Count() > 0)
                {
                    Console.WriteLine("Listagem dos alunos cadastrados no sistema\n");
                    var table = new ConsoleTable("R.A.", "Identificação", "Data de Aniversário", "E-mail");
                    foreach (var student in students)
                    {
                        table.AddRow(
                            student.Registration.ToString(),
                            student.Identification,
                            student.BirthDate.ToString("dd/MM/yyyy"),
                            student.Email
                        );
                    }
                    table.Write(Format.MarkDown);
                    Console.WriteLine("{0} registros encontrados.", students.Count());
                    Console.WriteLine("\nDesempenho da operação: {0:hh\\:mm\\:ss}.", stopwatch.Elapsed);
                    Console.WriteLine("SGBD: {0}.", connectedDataBase.ToString());
                    Console.WriteLine(Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Nenhum resultado encontrado.");
                    Console.WriteLine(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                System.Console.WriteLine("Erro do sistema: {0}.", ex.Message);
                System.Console.ResetColor();
                System.Console.WriteLine(System.Environment.NewLine);
            }
        }
        private static void SelectAllRequests()
        {
            try
            {
                RequestDAO requestDAO = new RequestDAO(connectedDataBase);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                ICollection<Request> requests = requestDAO.GetList();
                stopwatch.Stop();

                if (requests.Count() > 0)
                {
                    int totalCourseRequest = 0;
                    decimal totalCoursePrice = 0.0m;

                    Console.WriteLine("Listagem dos pedidos cadastrados no sistema\n");
                    var table = new ConsoleTable("Id", "Data do Pedido", "Aluno", "Qtd. Cursos", "Total do Pedido");
                    foreach (var request in requests)
                    {
                        table.AddRow(
                            request.Id.ToString(),
                            request.RequestDateTime.ToString("dd/MM/yyyy HH:mm"),
                            request.Student.Identification,
                            request.RequestDetails.Count(),
                            request.RequestDetails.Sum(x => x.Course.Price).ToString("C")
                        );

                        totalCourseRequest += request.RequestDetails.Count();
                        totalCoursePrice += request.RequestDetails.Sum(x => x.Course.Price);
                    }
                    table.Write(Format.MarkDown);
                    Console.WriteLine("{0} registros encontrados.", requests.Count());
                    Console.WriteLine("Quantidade total de cursos pedidos: {0}.", totalCourseRequest.ToString());
                    Console.WriteLine("Valor total dos cursos pedidos: {0}.", totalCoursePrice.ToString("C"));
                    Console.WriteLine("\nDesempenho da operação: {0:hh\\:mm\\:ss}.", stopwatch.Elapsed);
                    Console.WriteLine("SGBD: {0}.", connectedDataBase.ToString());
                    Console.WriteLine(Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Nenhum resultado encontrado.");
                    Console.WriteLine(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                System.Console.WriteLine("Erro do sistema: {0}.", ex.Message);
                System.Console.ResetColor();
                System.Console.WriteLine(System.Environment.NewLine);
            }
        }
        private static void SelectAllRequestDetails()
        {
            try
            {
                RequestDetailDAO requestDetailDAO = new RequestDetailDAO(connectedDataBase);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                ICollection<RequestDetail> requestDetails = requestDetailDAO.GetList();
                stopwatch.Stop();

                if (requestDetails.Count() > 0)
                {
                    int totalCourseRequest = requestDetails.Count();
                    decimal totalCoursePrice = 0.0m;

                    Console.WriteLine("Listagem dos detalhes dos pedidos cadastrados no sistema\n");
                    var table = new ConsoleTable("Id", "Aluno", "Data do Pedido", "Tipo do Curso", "Curso", "Preço");
                    foreach (var detail in requestDetails)
                    {
                        table.AddRow(
                            detail.Id,
                            detail.Request.Student.Identification,
                            detail.Request.RequestDateTime.ToString("dd/MM/yyy HH:mm"),
                            detail.Course.CourseType.Identification,
                            detail.Course.Identification,
                            detail.Course.Price.ToString("C")
                        );

                        totalCoursePrice += detail.Course.Price;
                    }
                    table.Write(Format.MarkDown);
                    Console.WriteLine("{0} registros encontrados.", requestDetails.Count());
                    Console.WriteLine("Quantidade total de cursos pedidos: {0}.", totalCourseRequest.ToString());
                    Console.WriteLine("Valor total dos cursos pedidos: {0}.", totalCoursePrice.ToString("C"));
                    Console.WriteLine("\nDesempenho da operação: {0:hh\\:mm\\:ss}.", stopwatch.Elapsed);
                    Console.WriteLine("SGBD: {0}.", connectedDataBase.ToString());
                    Console.WriteLine(Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Nenhum resultado encontrado.");
                    Console.WriteLine(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                System.Console.WriteLine("Erro do sistema: {0}.", ex.Message);
                System.Console.ResetColor();
                System.Console.WriteLine(System.Environment.NewLine);
            }
        }

        private static void SelectTotalCourseMinisteredByInstructor()
        {
            try
            {
                ReportDAO reportDAO = new ReportDAO(connectedDataBase);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                ICollection<Report> results = reportDAO.GetListCountGroupByInstructor();
                stopwatch.Stop();

                if (results.Count() > 0)
                {
                    int totalCourseMinister = 0;

                    Console.WriteLine("Relatório de total de cursos ministrados por instrutor\n");
                    var table = new ConsoleTable("Instrutor", "Qtd. Cursos");
                    foreach (var result in results)
                    {
                        table.AddRow(
                            result.InstructorIdentification,
                            result.TotalCourseMinistered.ToString()
                        );

                        totalCourseMinister += result.TotalCourseMinistered;
                    }
                    table.Write(Format.MarkDown);
                    Console.WriteLine("{0} registros encontrados.", results.Count());
                    Console.WriteLine("Quantidade total de cursos ministrados: {0}.", totalCourseMinister.ToString());
                    Console.WriteLine("\nDesempenho da operação: {0:hh\\:mm\\:ss}.", stopwatch.Elapsed);
                    Console.WriteLine("SGBD: {0}.", connectedDataBase.ToString());
                    Console.WriteLine(Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Nenhum resultado encontrado.");
                    Console.WriteLine(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                System.Console.WriteLine("Erro do sistema: {0}.", ex.Message);
                System.Console.ResetColor();
                System.Console.WriteLine(System.Environment.NewLine);
            }
        }
        private static void SelectTotalRequestCoursePriceByCourseType()
        {
            try
            {
                ReportDAO reportDAO = new ReportDAO(connectedDataBase);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                ICollection<Report> results = reportDAO.GetListSumGroupByCourseType();
                stopwatch.Stop();

                if (results.Count() > 0)
                {
                    int totalCourseRequest = 0;
                    decimal totalCoursePrice = 0.0m;

                    Console.WriteLine("Relatório de total de cursos pedidos por tipo de curso\n");
                    var table = new ConsoleTable("Tipo do Curso", "Qtd. Cursos", "Preço Total");
                    foreach (var result in results)
                    {
                        table.AddRow(
                            result.CourseTypeIdentification,
                            result.TotalCourseRequest.ToString(),
                            result.TotalCoursePrice.ToString("C")
                        );

                        totalCourseRequest += result.TotalCourseRequest;
                        totalCoursePrice += result.TotalCoursePrice;
                    }
                    table.Write(Format.MarkDown);
                    Console.WriteLine("{0} registros encontrados.", results.Count());
                    Console.WriteLine("Quantidade total de cursos pedidos: {0}.", totalCourseRequest.ToString());
                    Console.WriteLine("Valor total dos cursos pedidos: {0}.", totalCoursePrice.ToString("C"));
                    Console.WriteLine("\nDesempenho da operação: {0:hh\\:mm\\:ss}.", stopwatch.Elapsed);
                    Console.WriteLine("SGBD: {0}.", connectedDataBase.ToString());
                    Console.WriteLine(Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Nenhum resultado encontrado.");
                    Console.WriteLine(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                System.Console.WriteLine("Erro do sistema: {0}.", ex.Message);
                System.Console.ResetColor();
                System.Console.WriteLine(System.Environment.NewLine);
            }
        }
        private static void SelectTotalRequestCoursePriceByCourse()
        {
            try
            {
                ReportDAO reportDAO = new ReportDAO(connectedDataBase);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                ICollection<Report> results = reportDAO.GetListSumGroupByCourse();
                stopwatch.Stop();

                if (results.Count() > 0)
                {
                    int totalCourseRequest = 0;
                    decimal totalCoursePrice = 0.0m;

                    Console.WriteLine("Relatório de total de cursos pedidos por curso\n");
                    var table = new ConsoleTable("Curso", "Qtd. Cursos", "Preço Total");
                    foreach (var result in results)
                    {
                        table.AddRow(
                            result.CourseIdentification,
                            result.TotalCourseRequest.ToString(),
                            result.TotalCoursePrice.ToString("C")
                        );

                        totalCourseRequest += result.TotalCourseRequest;
                        totalCoursePrice += result.TotalCoursePrice;
                    }
                    table.Write(Format.MarkDown);
                    Console.WriteLine("{0} registros encontrados.", results.Count());
                    Console.WriteLine("Quantidade total de cursos pedidos: {0}.", totalCourseRequest.ToString());
                    Console.WriteLine("Valor total dos cursos pedidos: {0}.", totalCoursePrice.ToString("C"));
                    Console.WriteLine("\nDesempenho da operação: {0:hh\\:mm\\:ss}.", stopwatch.Elapsed);
                    Console.WriteLine("SGBD: {0}.", connectedDataBase.ToString());
                    Console.WriteLine(Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Nenhum resultado encontrado.");
                    Console.WriteLine(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                System.Console.WriteLine("Erro do sistema: {0}.", ex.Message);
                System.Console.ResetColor();
                System.Console.WriteLine(System.Environment.NewLine);
            }
        }
        private static void SelectTotalRequestCoursePriceByStudent()
        {
            try
            {
                ReportDAO reportDAO = new ReportDAO(connectedDataBase);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                ICollection<Report> results = reportDAO.GetListSumGroupByStudent();
                stopwatch.Stop();

                if (results.Count() > 0)
                {
                    int totalCourseRequest = 0;
                    decimal totalCoursePrice = 0.0m;

                    Console.WriteLine("Relatório de total de cursos pedidos por aluno\n");
                    var table = new ConsoleTable("Aluno", "Qtd. Cursos", "Preço Total");
                    foreach (var result in results)
                    {
                        table.AddRow(
                            result.StudentIdentification,
                            result.TotalCourseRequest.ToString(),
                            result.TotalCoursePrice.ToString("C")
                        );

                        totalCourseRequest += result.TotalCourseRequest;
                        totalCoursePrice += result.TotalCoursePrice;
                    }
                    table.Write(Format.MarkDown);
                    Console.WriteLine("{0} registros encontrados.", results.Count());
                    Console.WriteLine("Quantidade total de cursos pedidos: {0}.", totalCourseRequest.ToString());
                    Console.WriteLine("Valor total dos cursos pedidos: {0}.", totalCoursePrice.ToString("C"));
                    Console.WriteLine("\nDesempenho da operação: {0:hh\\:mm\\:ss}.", stopwatch.Elapsed);
                    Console.WriteLine("SGBD: {0}.", connectedDataBase.ToString());
                    Console.WriteLine(Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Nenhum resultado encontrado.");
                    Console.WriteLine(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                System.Console.WriteLine("Erro do sistema: {0}.", ex.Message);
                System.Console.ResetColor();
                System.Console.WriteLine(System.Environment.NewLine);
            }
        }

        private static void SelectCourseBestSeller()
        {
            try
            {
                ReportDAO reportDAO = new ReportDAO(connectedDataBase);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                Report result = reportDAO.GetCourseBestSeller();
                stopwatch.Stop();

                if (result != null)
                {
                    Console.WriteLine("{0} é o curso com maior número de pedidos.", result.CourseIdentification);
                    Console.WriteLine("\nDesempenho da operação: {0:hh\\:mm\\:ss}.", stopwatch.Elapsed);
                    Console.WriteLine("SGBD: {0}.", connectedDataBase.ToString());
                    Console.WriteLine(Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Nenhum resultado encontrado.");
                    Console.WriteLine(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                System.Console.WriteLine("Erro do sistema: {0}.", ex.Message);
                System.Console.ResetColor();
                System.Console.WriteLine(System.Environment.NewLine);
            }
        }
        private static void SelectStudentsRegisterInCourses()
        {
            try
            {
                ReportDAO reportDAO = new ReportDAO(connectedDataBase);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                ICollection<Report> results = reportDAO.GetListStudentsRegisterInCourses();
                stopwatch.Stop();

                if (results.Count() > 0)
                {
                    Console.WriteLine("Relatório de total de cursos pedidos por aluno\n");
                    var table = new ConsoleTable("R.A", "Aluno", "Curso", "Tipo do Curso", "Data da Matrícula");
                    foreach (var result in results)
                    {
                        table.AddRow(
                            result.StudentRegistration,
                            result.StudentIdentification,
                            result.CourseIdentification,
                            result.CourseTypeIdentification,
                            result.RequestDateTime
                        );
                    }
                    table.Write(Format.MarkDown);
                    Console.WriteLine("{0} registros encontrados.", results.Count());
                    Console.WriteLine("Quantidade total de cursos por alunos: {0}.", results.Count());
                    Console.WriteLine("\nDesempenho da operação: {0:hh\\:mm\\:ss}.", stopwatch.Elapsed);
                    Console.WriteLine("SGBD: {0}.", connectedDataBase.ToString());
                    Console.WriteLine(Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Nenhum resultado encontrado.");
                    Console.WriteLine(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                System.Console.WriteLine("Erro do sistema: {0}.", ex.Message);
                System.Console.ResetColor();
                System.Console.WriteLine(System.Environment.NewLine);
            }
        }
        private static void SelectStudentsWithoutRequests()
        {
            try
            {
                StudentDAO studentDAO = new StudentDAO(connectedDataBase);
                ReportDAO reportDAO = new ReportDAO(connectedDataBase);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                ICollection<Report> results = reportDAO.GetListStudentsWithoutRequests();
                stopwatch.Stop();

                if (results.Count() > 0)
                {
                    Console.WriteLine("Relatório de alunos que não possuem pedidos\n");
                    var table = new ConsoleTable("R.A", "Identificação");
                    foreach (var result in results)
                    {
                        table.AddRow(
                            studentDAO.GetByIdentification(result.StudentIdentification).Registration,
                            result.StudentIdentification
                        );
                    }
                    table.Write(Format.MarkDown);
                    Console.WriteLine("{0} registros encontrados.", results.Count());
                    Console.WriteLine("\nDesempenho da operação: {0:hh\\:mm\\:ss}.", stopwatch.Elapsed);
                    Console.WriteLine("SGBD: {0}.", connectedDataBase.ToString());
                    Console.WriteLine(Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Nenhum resultado encontrado.");
                    Console.WriteLine(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                System.Console.WriteLine("Erro do sistema: {0}.", ex.Message);
                System.Console.ResetColor();
                System.Console.WriteLine(System.Environment.NewLine);
            }
        }

        private static void AlterProviderStudentsEmail()
        {
            try
            {
                StudentDAO studentDAO = new StudentDAO(connectedDataBase);
                ICollection<Student> students = studentDAO.GetList();

                if (students.Count() > 0)
                {
                    Console.WriteLine("Aguarde enquanto o sistema atualiza os e-mails dos alunos.\n");
                    foreach (var student in students)
                    {
                        if (student.Email.Contains("@provider.com"))
                            student.Email = string.Format("{0}@provedor.com.br", student.Identification.Replace(" ", string.Empty).ToLower());
                        else
                            student.Email = string.Format("{0}@provider.com", student.Identification.Replace(" ", string.Empty).ToLower());
                    }

                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    studentDAO.Save(students);
                    stopwatch.Stop();

                    Console.WriteLine("{0} registros alterados com sucesso.", students.Count());
                    Console.WriteLine("\nDesempenho da operação: {0:hh\\:mm\\:ss}.", stopwatch.Elapsed);
                    Console.WriteLine("SGBD: {0}.", connectedDataBase.ToString());
                    Console.WriteLine(Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Nenhum resultado encontrado.");
                    Console.WriteLine(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                System.Console.WriteLine("Erro do sistema: {0}.", ex.Message);
                System.Console.ResetColor();
                System.Console.WriteLine(System.Environment.NewLine);
            }
        }

        private static void DeleteRequestsWithoutCourses()
        {
            try
            {
                RequestDAO requestDAO = new RequestDAO(connectedDataBase);
                ICollection<Request> requests = requestDAO.GetRequestsWithoutCourses();

                if (requests.Count != 0)
                {
                    Console.WriteLine("Aguarde enquanto o sistema excluí os pedidos.\n");

                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    requestDAO.Delete(requests);
                    stopwatch.Stop();

                    Console.WriteLine("{0} pedidos foram excluídos com sucesso do sistema.", requests.Count);
                    Console.WriteLine("\nDesempenho da operação: {0:hh\\:mm\\:ss}.", stopwatch.Elapsed);
                    Console.WriteLine("SGBD: {0}.", connectedDataBase.ToString());
                    Console.WriteLine(Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Nenhum resultado encontrado.");
                    Console.WriteLine(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                System.Console.WriteLine("Erro do sistema: {0}.", ex.Message);
                System.Console.ResetColor();
                System.Console.WriteLine(System.Environment.NewLine);
            }
        }
    }
}