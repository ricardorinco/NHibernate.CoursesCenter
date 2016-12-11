using NHibernate;
using NHibernate.Criterion;
using Persistence.Config;
using Persistence.POCO;
using System.Collections.Generic;

namespace Persistence.DAO
{
    public class RequestDetailDAO
    {
        private EntityManager entityManager = null;
        private ISessionFactory sessionFactory = null;

        public RequestDetailDAO(SGDB database)
        {
            entityManager = new EntityManager(database);
            sessionFactory = entityManager.GetSessionFactory();
        }

        public void Save(RequestDetail requestDetail)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            try
            {
                session.SaveOrUpdate(requestDetail);
            }
            catch (HibernateException ex)
            {
                transaction.Rollback();
                throw ex;
            }

            transaction.Commit();
            session.Close();
        }
        public void Save(ICollection<RequestDetail> requestDetails)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            int count = 1;
            foreach (var requestDetail in requestDetails)
            {
                try
                {
                    session.SaveOrUpdate(requestDetail);
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

        public void Delete(RequestDetail requestDetail)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();
            try
            {
                session.Delete(requestDetail);
            }
            catch (HibernateException ex)
            {
                transaction.Rollback();
                throw ex;
            }

            transaction.Commit();
            session.Close();
        }
        public void Delete(ICollection<RequestDetail> requestDetails)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            int count = 1;
            foreach (var requestDetail in requestDetails)
            {
                try
                {
                    session.Delete(requestDetail);
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

        public ICollection<RequestDetail> GetList()
        {
            ISession session = sessionFactory.OpenSession();

            try
            {
                ICollection<RequestDetail> requestDetails = session.CreateCriteria(typeof(RequestDetail))
                    .CreateAlias("Request", "Request")
                    .CreateAlias("Request.Student", "Student")
                    .CreateAlias("Course", "Course")
                    .CreateAlias("Course.CourseType", "CourseType")
                    .AddOrder(Order.Asc("Student.Identification"))
                    .AddOrder(Order.Desc("Request.RequestDateTime"))
                    .AddOrder(Order.Asc("CourseType.Identification"))
                    .AddOrder(Order.Asc("Course.Identification"))
                    .List<RequestDetail>();

                return requestDetails;
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