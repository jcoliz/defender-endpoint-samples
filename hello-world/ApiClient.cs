// Copyright (C) 2024 James Coliz, Jr. <jcoliz@outlook.com> All rights reserved
// Use of this source code is governed by the MIT license (see LICENSE.md)

using System.Net.Http.Headers;

namespace HellowWorld;

/// <summary>
/// Simple Api Client to make targeted requests to MDE API
/// </summary>
/// <param name="baseUri">Base service URI</param>
/// <param name="token">Auth token (JWT) from login</param>
public class ApiClient(Uri baseUri, string token)
{
    private readonly HttpClient client = new();

    /// <summary>
    /// Retrieve most recently-reported machines
    /// </summary>
    /// <returns>List of machines in JSON</returns>
    public Task<string> GetRecentMachinesAsync()
    {
        return GetAsync("machines?$top=5&$orderby=lastSeen desc");
    }

    /// <summary>
    /// Make the actual get request, and read the result as string
    /// </summary>
    /// <param name="pathAndQuery">Path and query component of URI</param>
    /// <returns></returns>
    private async Task<string> GetAsync(string pathAndQuery)
    {
        var uri = $"{baseUri}{pathAndQuery}";
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Authorization = new("Bearer", token);
        request.Headers.UserAgent.Add( new("defender-endpoint-samples","0.0.0") );

        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(response.ReasonPhrase);
        }

        var content = await response.Content.ReadAsStringAsync();

        return content;
    }
}
