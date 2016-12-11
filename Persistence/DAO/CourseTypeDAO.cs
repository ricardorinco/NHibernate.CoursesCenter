using NHibernate;
using NHibernate.Criterion;
using Persistence.Config;
using Persistence.POCO;
using System.Collections.Generic;

namespace Persistence.DAO
{
    public class CourseTypeDAO
    {
        private EntityManager entityManager = null;
        private ISessionFactory sessionFactory = null;

        public CourseTypeDAO(SGDB database)
        {
            entityManager = new EntityManager(database);
            sessionFactory = entityManager.GetSessionFactory();
        }

        public void Save(CourseType courseType)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();
                 
            try
            {
                session.SaveOrUpdate(courseType);
            }
            catch (HibernateException ex)
            {
                transaction.Rollback();
                throw ex;
            }

            transaction.Commit();
            session.Close();
        }
        public void Save(ICollection<CourseType> courseTypes)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            int count = 1;
            foreach (var courseType in courseTypes)
            {
                try
                {
                    session.SaveOrUpdate(courseType);
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

        public void Delete(CourseType courseType)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();
                 
            try
            {
                session.Delete(courseType);
            }
            catch (HibernateException ex)
            {
                transaction.Rollback();
                throw ex;
            }

            transaction.Commit();
            session.Close();
        }
        public void Delete(ICollection<CourseType> courseTypes)
        {
            ISession session = sessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            int count = 1;
            foreach (var courseType in courseTypes)
            {
                try
                {
                    session.Delete(courseType);
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

        public CourseType GetById(int id)
        {
            ISession session = sessionFactory.OpenSession();

            try
            {
                CourseType courseType = session.Get<CourseType>(id);

                return courseType;
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
        public CourseType GetByIdentification(string identification)
        {
            ISession session = sessionFactory.OpenSession();

            try
            {
                CourseType courseType = session.CreateCriteria(typeof(CourseType))
                    .Add(Expression.Eq("Identification", identification))
                    .UniqueResult<CourseType>();

                return courseType;
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

        public ICollection<CourseType> GetList()
        {
            ISession session = sessionFactory.OpenSession();

            try
            {
                ICollection<CourseType> coursesTypes = session.CreateCriteria(typeof(CourseType))
                        .List<CourseType>();

                return coursesTypes;
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