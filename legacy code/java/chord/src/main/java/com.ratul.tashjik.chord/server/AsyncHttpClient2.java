package com.ratul.tashjik.chord.server;


import org.asynchttpclient.AsyncHttpClient;
import org.asynchttpclient.Response;

import java.util.concurrent.CompletableFuture;

import static org.asynchttpclient.Dsl.*;

public class AsyncHttpClient2 {
    AsyncHttpClient httpClient = asyncHttpClient();

    public AsyncHttpClient2() {

    }

    public CompletableFuture<Response> httpGetAsync(String url) {
        return httpClient
                .prepareGet(url)
                .execute()
                .toCompletableFuture();
    }

    public CompletableFuture<Response> httpPostAsync(String url, String body) {
        return httpClient
                .preparePost(url)
                .setBody(body)
                .execute()
                .toCompletableFuture();
    }
}
