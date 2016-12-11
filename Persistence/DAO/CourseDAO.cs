using NHibernate;
using NHibernate.Criterion;
using Persistence.Config;
using Persistence.Helper;
using Persistence.POCO;
using System.Collections.Generic;

namespace Persistence.DAO
{
    public class CourseDAO
    {
        private EntityManager entityManager = null;
        private ISessionFactory sessionFactory = null;

        public CourseDAO(SGDB database)
        {
            entityManager = new EntityManager(database);
            sessionFactory = entityManager.GetSessionFactory();
        }

        public void Save(Course course)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            try
            {
                session.SaveOrUpdate(course);
            }
            catch (HibernateException ex)
            {
                transaction.Rollback();
                throw ex;
            }

            transaction.Commit();
            session.Close();
        }
        public void Save(ICollection<Course> courses)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            int count = 1;
            foreach (var course in courses)
            {
                try
                {
                    session.SaveOrUpdate(course);
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

        public void Delete(Course course)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            try
            {
                session.Delete(course);
            }
            catch (HibernateException ex)
            {
                transaction.Rollback();
                throw ex;
            }

            transaction.Commit();
            session.Close();
        }
        public void Delete(ICollection<Course> courses)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            int count = 1;
            foreach (var course in courses)
            {
                try
                {
                    session.Delete(course);
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

        public Course GetRandom()
        {
            ISession session = sessionFactory.OpenSession();

            try
            {
                int minimumId = 0;
                int maximumId = 0;

                minimumId = session.CreateCriteria(typeof(Course))
                    .SetMaxResults(1)
                    .AddOrder(Order.Asc("Id"))
                    .UniqueResult<Course>()
                    .Id;

                maximumId = session.CreateCriteria(typeof(Course))
                    .SetMaxResults(1)
                    .AddOrder(Order.Desc("Id"))
                    .UniqueResult<Course>()
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
        public Course GetById(int id)
        {
            ISession session = sessionFactory.OpenSession();

            try
            {
                Course course = session.Get<Course>(id);

                return course;
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

        public ICollection<Course> GetList()
        {
            ISession session = sessionFactory.OpenSession();

            try
            {
                ICollection<Course> courses = session.CreateCriteria(typeof(Course))
                    .AddOrder(Order.Asc("Id"))
                    .List<Course>();

                return courses;
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