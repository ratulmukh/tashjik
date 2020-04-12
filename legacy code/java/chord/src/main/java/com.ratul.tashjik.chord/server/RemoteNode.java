package com.ratul.tashjik.chord.server;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.asynchttpclient.Response;

import static com.ratul.tashjik.chord.server.ProtocolMsg.*;

import java.io.IOException;
import java.util.Optional;
import java.util.concurrent.CompletableFuture;

public class RemoteNode implements Node {
    private  final String ipAddress;
    AsyncHttpClient2 asyncHttpClient2 = new AsyncHttpClient2();
    ObjectMapper objectMapper = new ObjectMapper();

    public RemoteNode(String ipAddress) throws IOException {
        this.ipAddress = ipAddress;
    }


    private final String FIND_SUCCESSOR_ENDPOINT = "/findSuccessor";

    @Override
    public int getId() {
        return 0;
    }

    @Override
    public boolean isInInterval(Node node, int n) {
        return false;
    }

    @Override
    public boolean isInInterval(Node node1, Node node2) {
        return false;
    }

    @Override
    public CompletableFuture<Optional<RemoteNode>> findSuccessorAsync(int id) throws JsonProcessingException {
        String findSuccessorStr = objectMapper.writeValueAsString(new FindSucessorReq(id));

        return asyncHttpClient2.httpPostAsync(ipAddress + FIND_SUCCESSOR_ENDPOINT, findSuccessorStr)
                .thenApply(this::extractNodeFromResponse);

    }

    private Optional<RemoteNode> extractNodeFromResponse(Response response) {
        try {
            FindSucessorResp findSucessorResp = objectMapper.readValue(response.getResponseBody(), FindSucessorResp.class);
            return Optional.of(new RemoteNode(findSucessorResp.successorIp));
        } catch (IOException e) {
            e.printStackTrace();
            return Optional.empty();
        }
    }

    @Override
    public Boolean store(String key, String value) {
        return false;
    }
}
