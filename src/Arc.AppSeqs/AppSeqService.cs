// Copyright 2022 王建军
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


using NHibernate;

namespace Arc.AppSeqs;

public class AppSeqService : IAppSeqService
{
    readonly ISessionFactory _sessionFactory;

    public AppSeqService(ISessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    /// <summary>
    /// 获取下一个序号值，如果具有指定名称的序列不存在，则创建它。
    /// 序列值从 1 开始。
    /// </summary>
    /// <param name="seqName"></param>
    /// <returns></returns>
    public async Task<int> GetNextAsync(string seqName)
    {
        int next = 0;

        using (ISession session = _sessionFactory.OpenSession())
        {
            using (ITransaction tx = session.BeginTransaction())
            {
                var seq = await session
                    .GetAsync<AppSeq>(seqName, LockMode.Upgrade)
                    .ConfigureAwait(false);
                if (seq is null)
                {
                    seq = new AppSeq(seqName);
                    next = seq.GetNextVal();
                    await session
                        .SaveAsync(seq)
                        .ConfigureAwait(false);
                }
                else
                {
                    next = seq.GetNextVal();
                    await session
                        .UpdateAsync(seq)
                        .ConfigureAwait(false);
                }

                tx.Commit();
            }
        }

        return next;

    }
}
