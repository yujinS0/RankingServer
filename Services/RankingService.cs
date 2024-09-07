using RankingServer.Models;
using CloudStructures;
using CloudStructures.Structures;
using System.Threading.Tasks;
using StackExchange.Redis; // SortedSetOrder 사용을 위해 추가

namespace RankingServer.Services;

public class RankingService
{
    private readonly IDatabase _redisDb;
    private const string RedisSortedSetKey = "UserRanking";

    public RankingService(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    // 유저 점수 추가
    public async Task AddScoreAsync(UserScore userScore)
    {
        // Sorted Set에 닉네임과 점수를 추가
        await _redisDb.SortedSetAddAsync(RedisSortedSetKey, userScore.Nickname, userScore.Score);
    }

    // 랭킹 목록 가져오기 (상위 10명)
    public async Task<List<UserScore>> GetTopRankingAsync(int count = 10)
    {
        // 내림차순으로 상위 10명의 유저 가져오기
        var rankings = await _redisDb.SortedSetRangeByRankWithScoresAsync(RedisSortedSetKey, 0, count - 1, Order.Descending);

        return rankings.Select(x => new UserScore
        {
            Nickname = x.Element,
            Score = x.Score
        }).ToList();
    }
}