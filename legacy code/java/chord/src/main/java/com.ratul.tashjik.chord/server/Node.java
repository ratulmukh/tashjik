package com.ratul.tashjik.chord.server;

import com.fasterxml.jackson.core.JsonProcessingException;

import java.util.Optional;
import java.util.concurrent.CompletableFuture;

public interface Node {

    public int getId();
    public boolean isInInterval(Node node, int n);
    public boolean isInInterval(Node node1, Node node2);

    public CompletableFuture<Optional<RemoteNode>> findSuccessorAsync(int id) throws JsonProcessingException;

    public Boolean store(String key, String value);
}
