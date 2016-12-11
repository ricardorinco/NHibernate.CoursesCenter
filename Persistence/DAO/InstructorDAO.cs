using NHibernate;
using NHibernate.Criterion;
using Persistence.Config;
using Persistence.POCO;
using System.Collections.Generic;

namespace Persistence.DAO
{
    public class InstructorDAO
    {
        private EntityManager entityManager = null;
        private ISessionFactory sessionFactory = null;

        public InstructorDAO(SGDB database)
        {
            entityManager = new EntityManager(database);
            sessionFactory = entityManager.GetSessionFactory();
        }

        public void Save(Instructor instructor)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            try
            {
                session.SaveOrUpdate(instructor);
            }
            catch (HibernateException ex)
            {
                transaction.Rollback();
                throw ex;
            }

            transaction.Commit();
            session.Close();
        }
        public void Save(ICollection<Instructor> instructors)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            int count = 1;
            foreach (var instructor in instructors)
            {
                try
                {
                    session.SaveOrUpdate(instructor);
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

        public void Delete(Instructor instructor)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();
            try
            {
                session.Delete(instructor);
            }
            catch (HibernateException ex)
            {
                transaction.Rollback();
                throw ex;
            }

            transaction.Commit();
            session.Close();
        }
        public void Delete(ICollection<Instructor> instructors)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            int count = 1;
            foreach (var instructor in instructors)
            {
                try
                {
                    session.Delete(instructor);
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
        
        public ICollection<Instructor> GetList()
        {
            ISession session = sessionFactory.OpenSession();

            try
            {
                ICollection<Instructor> instructors = session.CreateCriteria(typeof(Instructor))
                    .AddOrder(Order.Asc("Identification"))
                    .List<Instructor>();

                return instructors;
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