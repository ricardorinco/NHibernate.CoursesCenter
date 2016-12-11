using NHibernate;
using NHibernate.Criterion;
using Persistence.Config;
using Persistence.POCO;
using System.Collections.Generic;

namespace Persistence.DAO
{
    public class RequestDAO
    {
        private EntityManager entityManager = null;
        private ISessionFactory sessionFactory = null;

        public RequestDAO(SGDB database)
        {
            entityManager = new EntityManager(database);
            sessionFactory = entityManager.GetSessionFactory();
        }

        public void Save(Request request)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            try
            {
                session.SaveOrUpdate(request);
            }
            catch (HibernateException ex)
            {
                transaction.Rollback();
                throw ex;
            }

            transaction.Commit();
            session.Close();
        }
        public void Save(ICollection<Request> requests)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            int count = 1;
            foreach (var request in requests)
            {
                try
                {
                    session.SaveOrUpdate(request);
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

        public void Delete(Request request)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();
            try
            {
                session.Delete(request);
            }
            catch (HibernateException ex)
            {
                transaction.Rollback();
                throw ex;
            }

            transaction.Commit();
            session.Close();
        }
        public void Delete(ICollection<Request> requests)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            int count = 1;
            foreach (var request in requests)
            {
                try
                {
                    session.Delete(request);
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

        public int GetLastId()
        {
            ISession session = sessionFactory.OpenSession();

            try
            {
                return session.CreateCriteria(typeof(Request))
                    .SetMaxResults(1)
                    .AddOrder(Order.Desc("Id"))
                    .UniqueResult<Request>()
                    .Id;
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

        // LazyLoad
        public ICollection<Request> GetList()
        {
            ISession session = sessionFactory.OpenSession();

            try
            {
                ICollection<Request> requests = session.CreateCriteria(typeof(Request))
                    .AddOrder(Order.Desc("RequestDateTime"))
                    .List<Request>();

                foreach (var request in requests)
                {
                    NHibernateUtil.Initialize(request.RequestDetails);
                }

                return requests;
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
        public ICollection<Request> GetRequestsWithoutCourses()
        {
            ISession session = sessionFactory.OpenSession();

            try
            {
                ICollection<Request> requestsWithoutCourses = session.CreateCriteria(typeof(Request))
                    .Add(Restrictions.IsEmpty("RequestDetails"))
                    .List<Request>();

                return requestsWithoutCourses;
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