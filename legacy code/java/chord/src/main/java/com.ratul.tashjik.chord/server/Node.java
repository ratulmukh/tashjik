package com.ratul.tashjik.chord.server;

import java.util.concurrent.CompletableFuture;

public interface Node {

    public int getId();
    public boolean isInInterval(Node node, int n);
    public boolean isInInterval(Node node1, Node node2);

    public CompletableFuture<Node> findSuccessorAsync(int id);

    public Boolean store(String key, String value);
}
