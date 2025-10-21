using System.Net.Http.Json;
using System.Text.Json;
using shared.Model;

namespace kreddit_app.Data;

public class ApiService
{
    private readonly HttpClient http;

    public ApiService(HttpClient http)
    {
        this.http = http;
    }

    public async Task<Post[]?> GetPosts()
    {
        return await http.GetFromJsonAsync<Post[]>("api/threads");
    }

    public async Task<Post?> GetPost(int id)
    {
        return await http.GetFromJsonAsync<Post>($"api/threads/{id}");
    }

    public async Task<bool> CreateComment(string content, int postId, string username)
    {
        try
        {
            var comment = new Comment { Content = content, User = new User { Username = username } };
            var response = await http.PostAsJsonAsync($"api/threads/{postId}/comments", comment);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating comment: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DownvotePost(int id)
    {
        var response = await http.PostAsync($"api/threads/{id}/downvote", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpvotePost(int id)
    {
        var responce = await http.PostAsync($"api/threads/{id}/upvote", null);
        return responce.IsSuccessStatusCode;
    }
    
    public async Task<bool> UpvoteComment(int commentId)
    {
        var response = await http.PostAsync($"api/comments/{commentId}/upvote", null);
        return response.IsSuccessStatusCode;
    }
    public async Task<bool> DownvoteComment(int commentId)
    {
        var response = await http.PostAsync($"api/comments/{commentId}/upvote", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<Post?> CreatePost(Post newPost)
    {
        var response = await http.PostAsJsonAsync("api/threads", newPost);
        if (!response.IsSuccessStatusCode)
        {
            
            var err = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Fejl CreatePost: {err}");
            return null;
        }
        var json = await response.Content.ReadAsStringAsync();
        var createdPost = JsonSerializer.Deserialize<Post>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return createdPost;
    }


    public async Task<Post[]> GetPostsSorted(string sortBy = "newest", string filterBy = "all")
    {
        return await http.GetFromJsonAsync<Post[]>($"api/threads/sorted?sortBy={sortBy}&filterBy={filterBy}") 
               ?? [];
    }
    
}