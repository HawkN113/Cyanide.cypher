﻿using System.Text;
using Cyanide.Cypher.Builders.Base;
using Cyanide.Cypher.Builders.Queries.General.Commands;
using Cyanide.Cypher.Extensions;

namespace Cyanide.Cypher.Builders.Queries.General;

internal sealed class CypherQueryBuilder() : BaseQueryBuilder(
        Enum.GetValues<QueryClauseKeys>().ToDictionary(key => key.GetDescription(), _ => new StringBuilder())), IQuery, IMatchQuery
{
    /// <summary>
    /// The WITH clause allows query parts to be chained together, piping the results from one to be used as starting points <br/>
    /// Sample: WITH otherPerson, count(*) AS foaf
    /// </summary>
    /// <param name="configureWith"></param>
    /// <returns></returns>
    public IWithQuery With(Action<WithClause> configureWith) =>
        ConfigureQueryBuilder<IWithQuery, WithClause>(
            QueryClauseKeys.With.GetDescription(), 
            configureWith);

    /// <summary>
    /// ORDER BY is a sub-clause following RETURN <br/>
    /// Sample: ORDER BY n.name
    /// </summary>
    /// <param name="configureOrderBy"></param>
    /// <returns></returns>
    public IOrderBySubQuery OrderBy(Action<OrderBySubClause> configureOrderBy) =>
        ConfigureQueryBuilder<IOrderBySubQuery, OrderBySubClause>(
            QueryClauseKeys.OrderBy.GetDescription(),
            configureOrderBy);

    /// <summary>
    /// LIMIT constrains the number of returned rows <br/>
    /// Sample: LIMIT 1 + toInteger(3 * rand())
    /// Sample: LIMIT 3
    /// </summary>
    /// <param name="configureLimit"></param>
    /// <returns></returns>
    public ILimitClause Limit(Action<LimitClause> configureLimit) =>
        ConfigureQueryBuilder<ILimitClause, LimitClause>(
            QueryClauseKeys.Limit.GetDescription(), 
            configureLimit);

    /// <summary>
    /// SKIP defines from which row to start including the rows in the output <br/>
    /// Sample: SKIP 1 + toInteger(3 * rand())
    /// Sample: SKIP 3
    /// </summary>
    /// <param name="configureSkip"></param>
    /// <returns></returns>
    public ISkipClause Skip(Action<SkipClause> configureSkip) =>
        ConfigureQueryBuilder<ISkipClause, SkipClause>(
            QueryClauseKeys.Skip.GetDescription(), 
            configureSkip);

    /// <summary>
    /// RETURN clause <br/>
    /// Sample: RETURN n.name
    /// </summary>
    /// <param name="configureReturn"></param>
    /// <returns></returns>
    public IReturnQuery Return(Action<ReturnClause> configureReturn) =>
        ConfigureQueryBuilder<IReturnQuery, ReturnClause>(
            QueryClauseKeys.Return.GetDescription(), 
            configureReturn);

    /// <summary>
    /// The SET clause is used to update labels on nodes and properties on nodes and relationships <br/>
    /// Sample: SET n.Name <br/>
    /// Sample: SET n.Name = 'Neo'
    /// </summary>
    /// <param name="configureSet"></param>
    /// <returns></returns>
    public ISetClause Set(Action<SetClause> configureSet) =>
        ConfigureQueryBuilder<ISetClause, SetClause>(
            QueryClauseKeys.Set.GetDescription(), 
            configureSet);

    /// <summary>
    /// The REMOVE clause is used to remove properties from nodes and relationships, and to remove labels from nodes. <br/>
    /// Sample: REMOVE n.Name
    /// </summary>
    /// <param name="configureRemove"></param>
    /// <returns></returns>
    public IRemoveQuery Remove(Action<RemoveClause> configureRemove) =>
        ConfigureQueryBuilder<IRemoveQuery, RemoveClause>(
            QueryClauseKeys.Remove.GetDescription(), 
            configureRemove);

    /// <summary>
    /// WHERE sub-clause <br/>
    /// Sample: WHERE n.name = 'Peter'
    /// </summary>
    /// <param name="conditions"></param>
    /// <returns></returns>
    public IWhereSubQuery Where(Action<WhereSubClause> conditions) =>
        ConfigureQueryBuilder<IWhereSubQuery, WhereSubClause>(
            QueryClauseKeys.Where.GetDescription(), 
            conditions);

    /// <summary>
    /// CREATE clause <br/>
    /// Sample: CREATE (n:Person)
    /// </summary>
    /// <param name="configureCreate"></param>
    /// <returns></returns>
    public ICreateQuery Create(Action<CreateClause> configureCreate) =>
        ConfigureQueryBuilder<ICreateQuery, CreateClause>(
            QueryClauseKeys.Create.GetDescription(), 
            configureCreate);

    /// <summary>
    /// MATCH clause <br/>
    /// Sample: MATCH (movie:Movie)
    /// </summary>
    /// <param name="configureMatch"></param>
    /// <returns></returns>
    public IMatchQuery Match(Action<MatchClause> configureMatch) =>
        ConfigureQueryBuilder<IMatchQuery, MatchClause>(
            QueryClauseKeys.Match.GetDescription(), 
            configureMatch);

    /// <summary>
    /// OPTIONAL MATCH clause does as MATCH <br/>
    /// Sample: OPTIONAL MATCH (p)-[r:DIRECTED]->()
    /// </summary>
    /// <param name="configureOptMatch"></param>
    /// <returns></returns>
    public IOptMatchQuery OptionalMatch(Action<OptMatchClause> configureOptMatch) =>
        ConfigureQueryBuilder<IOptMatchQuery, OptMatchClause>(
            QueryClauseKeys.OptionalMatch.GetDescription(),
            configureOptMatch);

    /// <summary>
    /// DELETE clause is used to delete nodes, relationships or paths <br/>
    /// Sample: DELETE r
    /// </summary>
    /// <param name="configureDelete"></param>
    /// <returns></returns>
    public IDeleteQuery Delete(Action<DeleteClause> configureDelete) =>
        ConfigureQueryBuilder<IDeleteQuery, DeleteClause>(
            QueryClauseKeys.Delete.GetDescription(), 
            configureDelete);

    /// <summary>
    /// DETACH DELETE clause may not be permitted to users with restricted security privileges <br/>
    /// Sample: DETACH DELETE n
    /// </summary>
    /// <param name="configureDelete"></param>
    /// <returns></returns>
    public IDetachDeleteQuery DetachDelete(Action<DetachDeleteClause> configureDelete) =>
        ConfigureQueryBuilder<IDetachDeleteQuery, DetachDeleteClause>(
            QueryClauseKeys.DetachDelete.GetDescription(),
            configureDelete);

    /// <summary>
    /// UNION clause is used to combine the result of multiple queries. <br/>
    /// Sample: MATCH (n:Actor) RETURN n.name AS name UNION MATCH (n:Movie) RETURN n.title AS name <br/>
    /// </summary>
    /// <param name="configureUnion"></param>
    /// <returns></returns>
    public IUnionClause Union(Action<IQuery> configureUnion)
    {
        var queryBuilder = new CypherQueryBuilder();
        configureUnion(queryBuilder);
        var unionClause = new StringBuilder("UNION ");
        unionClause.Append(queryBuilder.Build());
        allClauses[QueryClauseKeys.Union.ToString()] = unionClause;
        return this;
    }

    /// <summary>
    /// UNION ALL clause is used to combine the result of multiple queries. <br/>
    /// Sample: MATCH (n:Actor) RETURN n.name AS name UNION ALL MATCH (n:Movie) RETURN n.title AS name <br/>
    /// </summary>
    /// <param name="configureUnionAll"></param>
    /// <returns></returns>
    public IUnionClause UnionAll(Action<IQuery> configureUnionAll)
    {
        var queryBuilder = new CypherQueryBuilder();
        configureUnionAll(queryBuilder);
        var unionAllClause = new StringBuilder("UNION ALL ");
        unionAllClause.Append(queryBuilder.Build());
        allClauses[QueryClauseKeys.Union.ToString()] = unionAllClause;
        return this;
    }

    /// <summary>
    /// Generates the final Cypher query by concatenating all configured clauses.
    /// </summary>
    /// <returns>The fully constructed Cypher query as a string.</returns>
    public string Build()
    {
        var finalQuery = new StringBuilder();
        foreach (var clause in allClauses.Values.Where(clause => clause.Length > 0))
        {
            finalQuery.Append(clause);
            finalQuery.Append(' ');
        }

        return finalQuery.ToString().Trim();
    }
}