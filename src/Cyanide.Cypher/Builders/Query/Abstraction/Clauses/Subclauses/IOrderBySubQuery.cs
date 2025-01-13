﻿using Cyanide.Cypher.Builders.Query.Commands;

namespace Cyanide.Cypher.Builders.Query
{
    public interface IOrderBySubQuery: ISkipClause
    {
        /// <summary>
        /// ORDER BY is a sub-clause following RETURN <br/>
        /// Sample: ORDER BY n.name
        /// </summary>
        /// <param name="configureOrderBy"></param>
        /// <returns></returns>
        IOrderBySubQuery OrderBy(Action<OrderBySubClause> configureOrderBy);
    }
}