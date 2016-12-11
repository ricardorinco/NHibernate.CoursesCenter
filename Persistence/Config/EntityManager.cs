using NHibernate;
using NHibernate.Cfg;
using Persistence.POCO;
using System;
using System.Reflection;

namespace Persistence.Config
{
    public enum SGDB { SQLServer, MySQL }

    public class EntityManager : IDisposable
    {
        private ISessionFactory sessionFactory = null;

        public EntityManager(SGDB database)
        {
            // Inicialização
            Configuration configuration = new Configuration();
            
            if (database == SGDB.SQLServer)
                configuration.Configure("Hibernate_SQL.cfg.xml");
            else if (database == SGDB.MySQL)
                configuration.Configure("Hibernate_MySQL.cfg.xml");

            // Adicionando o mapeamento para configuração do objeto
            Assembly assembly = typeof(CourseType).Assembly;
            configuration.AddAssembly(assembly);

            // Criando a seção factory para configuração do objeto
            sessionFactory = configuration.BuildSessionFactory();
        }

        public ISessionFactory GetSessionFactory()
        {
            return sessionFactory;
        }
        public void Dispose()
        {
            sessionFactory.Dispose();
        }
    }
}
