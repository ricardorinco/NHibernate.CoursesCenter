using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Persistence.Config;
using Persistence.POCO;
using System.Collections.Generic;
using System.Text;

namespace Persistence.DAO
{
    public class ReportDAO
    {
        private EntityManager entityManager = null;
        private ISessionFactory sessionFactory = null;

        public ReportDAO(SGDB database)
        {
            entityManager = new EntityManager(database);
            sessionFactory = entityManager.GetSessionFactory();
        }

        public Report GetCourseBestSeller()
        {
            ISession session = sessionFactory.OpenSession();

            try
            {
                var courseBestSeller = session.CreateCriteria(typeof(RequestDetail))
                    .CreateAlias("Course", "Course")
                    .SetProjection(
                        Projections.ProjectionList()
                        .Add(Projections.GroupProperty("Course.Identification"), "CourseIdentification")
                        .Add(Projections.Count("Course"), "CourseBestSeller")
                    )
                    .AddOrder(Order.Desc("CourseBestSeller"))
                    .SetResultTransformer(Transformers.AliasToBean<Report>())
                    .SetMaxResults(1);

                return courseBestSeller.UniqueResult<Report>();
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

        public ICollection<Report> GetListStudentsRegisterInCourses()
        {
            ISession session = sessionFactory.OpenSession();

            StringBuilder hql = new StringBuilder();
            hql.AppendLine("select s.Registration as StudentRegistration,");
            hql.AppendLine("	   s.Identification as StudentIdentification,");
            hql.AppendLine("	   c.Identification as CourseIdentification,");
            hql.AppendLine("       ct.Identification as CourseTypeIdentification,");
            hql.AppendLine("       r.RequestDateTime");
            hql.AppendLine("  from RequestDetail rq");
            hql.AppendLine(" inner join Request r on r.Id = rq.RequestId");
            hql.AppendLine(" inner join Course c on c.Id = rq.CourseId");
            hql.AppendLine(" inner join CourseType ct on ct.Id = c.CourseTypeId");
            hql.AppendLine(" inner join Student s on s.Id = r.StudentId");
            hql.AppendLine(" order by StudentRegistration, StudentIdentification, CourseIdentification, CourseTypeIdentification");

            try
            {
                ICollection<Report> studentsRegisterInCourses = session.CreateSQLQuery(hql.ToString())
                    .SetResultTransformer(Transformers.AliasToBean<Report>())
                    .List<Report>();

                return studentsRegisterInCourses;
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
        public ICollection<Report> GetListStudentsWithoutRequests()
        {
            ISession session = sessionFactory.OpenSession();

            try
            {
                var studentsWithoutRequests = session.CreateCriteria(typeof(Student))
                    .CreateAlias("Requests", "Request", NHibernate.SqlCommand.JoinType.LeftOuterJoin)
                    .SetProjection(
                        Projections.ProjectionList()
                        .Add(Projections.GroupProperty("Identification"), "StudentIdentification")
                    )
                    .Add(Restrictions.Eq(Projections.Count("Request.Id"), 0))
                    .SetResultTransformer(Transformers.AliasToBean<Report>());

                return studentsWithoutRequests.List<Report>();
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
        public ICollection<Report> GetListCountGroupByInstructor()
        {
            ISession session = sessionFactory.OpenSession();

            try
            {
                var countGroupByCourse = session.CreateCriteria(typeof(Course))
                    .CreateAlias("Instructor", "Instructor", NHibernate.SqlCommand.JoinType.LeftOuterJoin)
                    .SetProjection(
                        Projections.ProjectionList()
                        .Add(Projections.GroupProperty("Instructor.Identification"), "InstructorIdentification")
                        .Add(Projections.Count("Id"), "TotalCourseMinistered")
                    )
                    .AddOrder(Order.Desc("TotalCourseMinistered"))
                    .SetResultTransformer(Transformers.AliasToBean<Report>());

                return countGroupByCourse.List<Report>();
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
        public ICollection<Report> GetListSumGroupByCourseType()
        {
            ISession session = sessionFactory.OpenSession();

            try
            {
                var sumGroupByCourseType = session.CreateCriteria(typeof(RequestDetail))
                    .CreateAlias("Course", "Course")
                    .CreateAlias("Course.CourseType", "CourseType")
                    .SetProjection(
                        Projections.ProjectionList()
                        .Add(Projections.GroupProperty("CourseType.Identification"), "CourseTypeIdentification")
                        .Add(Projections.Count("Course"), "TotalCourseRequest")
                        .Add(Projections.Sum("Course.Price"), "TotalCoursePrice")
                    )
                    .AddOrder(Order.Asc("CourseTypeIdentification"))
                    .SetResultTransformer(Transformers.AliasToBean<Report>());

                return sumGroupByCourseType.List<Report>();
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
        public ICollection<Report> GetListSumGroupByCourse()
        {
            ISession session = sessionFactory.OpenSession();

            try
            {
                var sumGroupByCourse = session.CreateCriteria(typeof(RequestDetail))
                    .CreateAlias("Course", "Course", NHibernate.SqlCommand.JoinType.LeftOuterJoin)
                    .SetProjection(
                        Projections.ProjectionList()
                        .Add(Projections.GroupProperty("Course.Identification"), "CourseIdentification")
                        .Add(Projections.Count("Course"), "TotalCourseRequest")
                        .Add(Projections.Sum("Course.Price"), "TotalCoursePrice")
                    )
                    .AddOrder(Order.Asc("CourseIdentification"))
                    .SetResultTransformer(Transformers.AliasToBean<Report>());

                return sumGroupByCourse.List<Report>();
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
        public ICollection<Report> GetListSumGroupByStudent()
        {
            ISession session = sessionFactory.OpenSession();

            try
            {
                var sumGroupByStudent = session.CreateCriteria(typeof(RequestDetail))
                    .CreateAlias("Request", "Request")
                    .CreateAlias("Request.Student", "Student")
                    .CreateAlias("Course", "Course")
                    .SetProjection(
                        Projections.ProjectionList()
                        .Add(Projections.GroupProperty("Student.Identification"), "StudentIdentification")
                        .Add(Projections.Count("Course"), "TotalCourseRequest")
                        .Add(Projections.Sum("Course.Price"), "TotalCoursePrice")
                    )
                    .AddOrder(Order.Asc("StudentIdentification"))
                    .SetResultTransformer(Transformers.AliasToBean<Report>());

                return sumGroupByStudent.List<Report>();
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
