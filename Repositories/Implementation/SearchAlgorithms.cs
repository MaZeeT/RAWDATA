using Domain.Models;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Implementation;

public static class SearchAlgorithms
{
    public static class Tfidf
    {
        internal static List<Search> List(DatabaseContext2 db, string[] searchWords)
        {
            return Query(db, searchWords).ToList();
        }

        internal static int Count(DatabaseContext2 db, string[] searchWords)
        {
            return Query(db, searchWords).Count();
        }

        private static IEnumerable<Search> Query(DatabaseContext2 db, string[] searchWords)
        {
            // Base query from wi_weighted table
            var query = db.WiWeighted
                .Where(w => (w.What == "title" || w.What == "body")
                            && searchWords.Contains(w.Word));

            // Step 1: Let EF compute the sum (no rounding yet)
            var intermediate = query
                .GroupBy(w => w.Id)
                .Select(g => new
                {
                    PostId = g.Key,
                    Rank = (double)(g.Sum(x => x.Tfidf) ?? 0)
                })
                .OrderByDescending(r => r.Rank)
                .ToList();

            // Step 2: Perform rounding in memory (safe and unambiguous)
            var results = intermediate
                .Select(r => new Search
                {
                    PostId = r.PostId,
                    Rank = Math.Round(r.Rank, 4)
                });

            return results;
        }
    }

    public static class ExactMatch
    {
        internal static List<Search> List(DatabaseContext2 db, string[] searchWords)
        {
            return Query(db, searchWords).ToList();
        }

        internal static int Count(DatabaseContext2 db, string[] searchWords)
        {
            return Query(db, searchWords).Count();
        }

        private static IQueryable<Search> Query(DatabaseContext2 db, string[] searchWords)
        {
            if (searchWords.Length == 0)
                return Enumerable.Empty<Search>().AsQueryable();

            IQueryable<int> answerIds;
            IQueryable<int> questionIds;

            if (searchWords.Length == 1)
            {
                var kw = searchWords[0];

                answerIds = from a in db.Answers
                    join w in db.WiWeighted on a.Id equals w.Id
                    where w.Word == kw
                    select a.Id;

                questionIds = from q in db.Questions
                    join w in db.WiWeighted on q.Id equals w.Id
                    where w.Word == kw
                    select q.Id;
            }
            else
            {
                // Multiple keywords -> INTERSECT logic
                answerIds = db.WiWeighted.Where(w => w.Word == searchWords[0])
                    .Select(w => w.Id);

                foreach (var kw in searchWords.Skip(1))
                {
                    answerIds = answerIds.Intersect(
                        db.WiWeighted.Where(w => w.Word == kw).Select(w => w.Id)
                    );
                }

                questionIds = db.WiWeighted.Where(w => w.Word == searchWords[0])
                    .Select(w => w.Id);

                foreach (var kw in searchWords.Skip(1))
                {
                    questionIds = questionIds.Intersect(
                        db.WiWeighted.Where(w => w.Word == kw).Select(w => w.Id)
                    );
                }

                // Join filtered ids with answers/questions
                answerIds = from a in db.Answers
                    join id in answerIds on a.Id equals id
                    select a.Id;

                questionIds = from q in db.Questions
                    join id in questionIds on q.Id equals id
                    select q.Id;
            }

            // Build final Search result
            var answersQuery = answerIds.Select(id => new Search
            {
                PostId = id,
                Rank = (double)0m
            });

            var questionsQuery = questionIds.Select(id => new Search
            {
                PostId = id,
                Rank = (double)0m
            });

            return answersQuery.Concat(questionsQuery);
        }
    }

    public static class SimpleSearch
    {
        internal static List<Search> List(DatabaseContext2 db, string[] searchWords)
        {
            return Query(db, searchWords).ToList();
        }

        internal static int Count(DatabaseContext2 db, string[] searchWords)
        {
            return Query(db, searchWords).Count();
        }

        private static IQueryable<Search> Query(DatabaseContext2 db, string[] searchWords)
        {
            if (searchWords.Length == 0)
                return new List<Search>().AsQueryable();

            var keyword = searchWords[0];

            // Wrap keyword with % for substring search (ILike = case-insensitive)
            var pattern = $"%{keyword}%";

            var questionMatches = db.Questions
                .Where(q =>
                    EF.Functions.ILike(q.Title, pattern) ||
                    EF.Functions.ILike(q.Body, pattern))
                .Select(q => new Search
                {
                    PostId = q.Id,
                    Rank = (double)0m
                });

            var answerMatches = db.Answers
                .Where(a => EF.Functions.ILike(a.Body, pattern))
                .Select(a => new Search
                {
                    PostId = a.Id,
                    Rank = (double)0m
                });

            // UNION ALL equivalent
            return questionMatches
                .Union(answerMatches);
        }
    }

    public static class BestMatch
    {
        internal static List<Search> List(DatabaseContext2 db, string[] searchWords)
        {
            return Query(db, searchWords).ToList();
        }

        internal static int Count(DatabaseContext2 db, string[] searchWords)
        {
            return Query(db, searchWords).Count();
        }

        private static IQueryable<Search> Query(DatabaseContext2 db, string[] searchWords)
        {
            if (searchWords.Length == 0)
                return Enumerable.Empty<Search>().AsQueryable();

            // Combine questions and answers
            var posts = db.Questions
                .Select(q => new { q.Id, q.Body })
                .Concat(db.Answers.Select(a => new { a.Id, a.Body }));

            // Build relevance table for all keywords
            var relevanceQuery = db.WiWeighted
                .Where(w => searchWords.Contains(w.Word))
                .Select(w => new { w.Id, Relevance = 1 });

            // Join posts with relevance
            var query = from p in posts
                join r in relevanceQuery on p.Id equals r.Id
                group r by p.Id
                into g
                select new Search
                {
                    PostId = g.Key,
                    Rank = (double)(g.Sum(x => (decimal?)x.Relevance) ?? 0m)
                };

            return query.OrderByDescending(s => s.Rank);
        }
    }
    
}