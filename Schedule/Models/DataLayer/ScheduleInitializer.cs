using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Schedule.Models.DataLayer
{
    class ScheduleInitializer : DropCreateDatabaseAlways<ScheduleDbContext>
    {
        protected override void Seed(ScheduleDbContext context)
        {
            var schools = new List<School>
            {
                new School {Name = "ШЕН"},
                new School {Name = "ИШ"}
            };

            var buildings = new List<Building>
            {
                new Building {Name = "C"},
                new Building {Name = "D"},
                new Building {Name = "S"}
            };

            buildings[0].Schools.Add(schools[1]);
            buildings[1].Schools.Add(schools[0]);

            var rooms = new List<Classroom>
            {
                new Classroom {Number = 738, Type = ClassroomType.LectionClass, Capacity = 200},
                new Classroom {Number = 733, Type = ClassroomType.ComputerClass, Capacity = 40},
                new Classroom {Number = 818, Type = ClassroomType.PracticeClass, Capacity = 30},
                new Classroom {Number = 941, Type = ClassroomType.ComputerClass, Capacity = 15},
                new Classroom {Number = 1, Type = ClassroomType.Gym, Capacity = 500},
                new Classroom {Number = 2, Type = ClassroomType.Gym, Capacity = 500}
            };

            for (int i = 0; i < 4; i++)
                buildings[1].Classrooms.Add(rooms[i]);
            for (int i = 4; i < 6; i++)
                buildings[2].Classrooms.Add(rooms[i]);

            var teachers = new List<Teacher>
            {
                new Teacher {Name = "Пак Геннадий Константинович"},
                new Teacher {Name = "Шепелева Риорита Петровна"},
                new Teacher {Name = "Клевчихин Юрий Александрович"},
                new Teacher {Name = "Кленин Александр Сергеевич"},
                new Teacher {Name = "Туфанов Игорь Евгеньевич"}
            };

            var courses = new List<Course>
            {
                new Course {Name = "Математический анализ"},
                new Course {Name = "Функциональный анализ"},
                new Course {Name = "Дифференциальные уравнения"},
                new Course {Name = "Алгебра и геометрия"},
                new Course {Name = "Дискретная математика"},
                new Course {Name = "Практикум на ЭВМ"},
                new Course {Name = "Введение в базы данных"},
                new Course {Name = "Алгоритмы"}
            };

            var faculties = new List<Faculty>
            {
                new Faculty {Name = "Кафедра алгебры и логики"},
                new Faculty {Name = "Кафедра математического анализа"},
                new Faculty {Name = "Кафедра информатики"}
            };

            faculties[0].Teachers.Add(teachers[0]);
            faculties[1].Teachers.Add(teachers[1]);
            faculties[1].Teachers.Add(teachers[2]);
            faculties[2].Teachers.Add(teachers[3]);
            faculties[2].Teachers.Add(teachers[4]);

            for (int i = 3; i < 5; i++)
                teachers[0].Courses.Add(courses[i]);
            for (int i = 0; i < 3; i++)
            {
                teachers[1].Courses.Add(courses[i]);
                teachers[2].Courses.Add(courses[i]);
            }
            for (int i = 5; i < 7; i++)
                teachers[3].Courses.Add(courses[i]);
            teachers[4].Courses.Add(courses[7]);

            foreach (var f in faculties)
                schools[0].Faculties.Add(f);

            var students = new List<Student>
            {
                new Student {Name = "Александров Никита Александрович"},
                new Student {Name = "Бабич Михаил Викторович"},
                new Student {Name = "Блинов Игорь Олегович"},
                new Student {Name = "Болотин Иван Андреевич"},
                new Student {Name = "Волкогонова Юлия Викторовна"},
                new Student {Name = "Декальчук Анастасия Александровна"},
                new Student {Name = "Добрынин Никита Викторович"},
                new Student {Name = "Зиман Даниил Игоревич"},
                new Student {Name = "Зубарев Павел Сергеевич"},
                new Student {Name = "Кравчук Роман Романович"},
                new Student {Name = "Кузнецов Игорь Русланович"},
                new Student {Name = "Куницкий Алексей Алексеевич"},
                new Student {Name = "Мироедов Владислав Александрович"},
                new Student {Name = "Павлов Михаил Александрович"},
                new Student {Name = "Савинов Павел Алексеевич"},
                new Student {Name = "Суренков Савва Павлович"},
                new Student {Name = "Туров Дмитрий Романович"},
                new Student {Name = "Цой Алексей Александрович"},
                new Student {Name = "Швецова Анна Николаевна"},
                new Student {Name = "Шишиморов Дмитрий Сергеевич"},
                new Student {Name = "Ян Татьяна Вячеславовна"}
            };

            var group = new Group { Name = "Б8103а" };
            foreach (var s in students)
                group.Students.Add(s);

            faculties[2].Groups.Add(group);

            var scheduleItems = new List<Schedule>
            {
                new Schedule
                {
                    Course = courses[0], 
                    Type = CourseType.Lection, 
                    Teacher = teachers[2], 
                    Group = group,
                    StartDate = new DateTime(2014, 9, 1), 
                    EndDate = new DateTime(2014, 12, 29), 
                    Interval = 7,
                    DoubleClass = DoubleClass.First,
                    Class = rooms[0]
                },
                new Schedule
                {
                    Course = courses[0], 
                    Type = CourseType.Practice, 
                    Teacher = teachers[2], 
                    Group = group,
                    StartDate = new DateTime(2014, 9, 1), 
                    EndDate = new DateTime(2014, 12, 29), 
                    Interval = 14,
                    DoubleClass = DoubleClass.Second,
                    Class = rooms[2]
                }
            };

            context.Buildings.AddRange(buildings);

            foreach (var item in scheduleItems)
                group.Schedule.Add(item);

            // Save initialized context
            context.SaveChanges();

            base.Seed(context);
        }
    }
}