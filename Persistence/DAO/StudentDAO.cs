using NHibernate;
using NHibernate.Criterion;
using Persistence.Config;
using Persistence.Helper;
using Persistence.POCO;
using System.Collections.Generic;

namespace Persistence.DAO
{
    public class StudentDAO
    {
        private EntityManager entityManager = null;
        private ISessionFactory sessionFactory = null;

        public StudentDAO(SGDB database)
        {
            entityManager = new EntityManager(database);
            sessionFactory = entityManager.GetSessionFactory();
        }

        public void Save(Student student)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            try
            {
                session.SaveOrUpdate(student);
            }
            catch (HibernateException ex)
            {
                transaction.Rollback();
                throw ex;
            }

            transaction.Commit();
            session.Close();
        }
        public void Save(ICollection<Student> students)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            int count = 1;
            foreach (var student in students)
            {
                try
                {
                    session.SaveOrUpdate(student);
                    count++;

                    if (count % 20 == 0)
                    {
                        session.Flush();
                        session.Clear();
                    }
                }
                catch (HibernateException ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

            transaction.Commit();
            session.Close();
        }

        public void Delete(Student student)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();
            try
            {
                session.Delete(student);
            }
            catch (HibernateException ex)
            {
                transaction.Rollback();
                throw ex;
            }

            transaction.Commit();
            session.Close();
        }
        public void Delete(ICollection<Student> students)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            int count = 1;
            foreach (var student in students)
            {
                try
                {
                    session.Delete(student);
                    count++;

                    if (count % 20 == 0)
                    {
                        session.Flush();
                        session.Clear();
                    }
                }
                catch (HibernateException ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

            transaction.Commit();
            session.Close();
        }

        public Student GetRandom()
        {
            ISession session = sessionFactory.OpenSession();

            try
            {
                int minimumId = 0;
                int maximumId = 0;

                minimumId = session.CreateCriteria(typeof(Student))
                    .SetMaxResults(1)
                    .AddOrder(Order.Asc("Id"))
                    .UniqueResult<Student>()
                    .Id;

                maximumId = session.CreateCriteria(typeof(Student))
                    .SetMaxResults(1)
                    .AddOrder(Order.Desc("Id"))
                    .UniqueResult<Student>()
                    .Id;

                return GetById(Aleatory.GetInteger(minimumId, maximumId));
            }
            catch (HibernateException ex)
            {
                throw ex;
            }
            finally
            {
                session.Close();
            }
        }
        public Student GetById(int id)
        {
            ISession session = sessionFactory.OpenSession();

            try
            {
                Student student = session.Get<Student>(id);

                return student;
            }
            catch (HibernateException ex)
            {
                throw ex;
            }
            finally
            {
                session.Close();
            }
        }
        public Student GetByIdentification(string identification)
        {
            ISession session = sessionFactory.OpenSession();

            try
            {
                Student student = session.CreateCriteria(typeof(Student))
                    .Add(Expression.Eq("Identification", identification))
                    .UniqueResult<Student>();

                return student;
            }
            catch (HibernateException ex)
            {
                throw ex;
            }
            finally
            {
                session.Close();
            }
        }

        public ICollection<Student> GetList()
        {
            ISession session = sessionFactory.OpenSession();

            try
            {
                ICollection<Student> students = session.CreateCriteria(typeof(Student))
                    .AddOrder(Order.Asc("Identification"))
                    .List<Student>();

                return students;
            }
            catch (HibernateException ex)
            {
                throw ex;
            }
            finally
            {
                session.Close();
            }
        }
    }
}