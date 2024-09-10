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

    //// 유저 점수 추가 (기본)
    //public async Task AddScoreAsync(UserScore userScore)
    //{
    //    // Sorted Set에 닉네임과 점수를 추가
    //    await _redisDb.SortedSetAddAsync(RedisSortedSetKey, userScore.Nickname, userScore.Score);
    //}

    // 유저 점수 추가 (최고 기록일 때만 업데이트)
    public async Task<bool> AddScoreAsync(UserScore userScore)
    {
        // 현재 해당 닉네임의 점수 조회
        var currentScore = await _redisDb.SortedSetScoreAsync(RedisSortedSetKey, userScore.Nickname);

        // 기존 점수가 없는 경우 또는 새 점수가 더 클 때만 업데이트
        if (currentScore == null || userScore.Score >= currentScore)
        {
            await _redisDb.SortedSetAddAsync(RedisSortedSetKey, userScore.Nickname, userScore.Score);
            return true; // 점수가 업데이트됨
        }
        else
        {
            return false; // 더 낮은 점수이므로 업데이트하지 않음
        }
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