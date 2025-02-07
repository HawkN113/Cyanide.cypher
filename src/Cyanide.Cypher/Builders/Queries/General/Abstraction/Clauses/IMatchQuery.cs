﻿using Cyanide.Cypher.Attributes;
using Cyanide.Cypher.Builders.Queries.General.Commands;

namespace Cyanide.Cypher.Builders.Queries.General;

public interface IMatchQuery: IOptMatchQuery, IDeleteQuery, IDetachDeleteQuery, IRemoveQuery, ISetClause, IWithQuery
{
    /// <summary>
    /// MATCH clause <br/>
    /// Sample: MATCH (movie:Movie)
    /// </summary>
    /// <param name="configureMatch"></param>
    /// <returns></returns>
    [VersionInfo("4.4")]
    IMatchQuery Match(Action<MatchClause> configureMatch);
}