using System;
using System.Collections.Generic;
using System.Threading;

namespace BarSimulator
{
    class Bar
    {
        List<Student> students = new List<Student>();
        Semaphore semaphore = new Semaphore(10, 10);
        Random random = new Random();
        public bool isOpen = true;
        object barLock = new object();

        public BarStatus Enter(Student student)
        {
            if (!isOpen) return BarStatus.Closed;
            lock (barLock)
            {
                StillOpen();
            }
            semaphore.WaitOne();
            lock (students)
            {
                if (!isOpen) return BarStatus.Closed;
                if (student.Age < 18) return BarStatus.Underage;
                students.Add(student);
                return BarStatus.CanEnter;
            }
        }

        public void Closing()
        {
            Console.WriteLine($"========== {students.Count} visitors will leave.==========");
            lock (students)
            {
                foreach (Student student in students)
                {
                    student.LeaveBar();
                    semaphore.Release();
                }
                students.Clear();
            }
            Console.WriteLine("==========CLOSED==========");
        }

        public void StillOpen()
        {
            int x = random.Next(1, 20);
            if (x >= 18 && isOpen)
            {
                Console.WriteLine("==========GET OUT!==========");
                isOpen = false;
                Thread.Sleep(100);
                Closing();
            }
        }

        public void Leave(Student student)
        {
            lock (students)
            {
                students.Remove(student);
            }
            semaphore.Release();
        }
    }
}
