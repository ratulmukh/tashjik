package com.ratul.tashjik.chord.clients;


import com.ratul.tashjik.chord.server.LocalNode;
import com.ratul.tashjik.chord.server.Node;
import com.ratul.tashjik.chord.server.Utils;
import org.apache.commons.lang3.NotImplementedException;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutionException;
import java.util.stream.Collectors;

public class StorageClient {
    private int REPLICATION_COUNT = 3;
    private LocalNode localNode = new LocalNode(null);

    public StorageClient() throws ExecutionException, InterruptedException {
    }

    public String getValue(String key) {
        throw new NotImplementedException("getValue");
    }

    public CompletableFuture<String> getValueAsync(String  key) {
        throw new NotImplementedException("getValueAsync");
    }

    public void putKeyValue(String key, String value) {
        throw new NotImplementedException("putKeyValue");
    }

    public CompletableFuture<Boolean> putKeyValueAsync(String  key, String value) {
        List<Integer> ids = new ArrayList<>(REPLICATION_COUNT);
        for (int i=0; i<REPLICATION_COUNT; i++) {
            ids.add(Utils.createId(key));
        }

        List<CompletableFuture<Boolean>> storageResults = ids.stream()
                .map(id -> CompletableFuture.completedFuture(id)
                            .thenApply(id1 -> localNode.findSuccessorAsync(id1))
                            .thenApply(successorFuture -> {return new StorageIntent(successorFuture, key, value);})
                            .thenApply(this::store))
                .collect(Collectors.toList());


        CompletableFuture<Void> allFutures = CompletableFuture.allOf(storageResults.toArray(new CompletableFuture[REPLICATION_COUNT]));

        //unfortunate code below to account for erasure coding in above statement
        return allFutures.thenApply(v -> {
                    return storageResults.stream()
                        .map(storageResult -> storageResult.join())
                        .collect(Collectors.toList());})
                .thenApply(this::orBools);


    }

    private boolean orBools(List<Boolean>boolList) {
        if(boolList.stream().filter(boolVal -> boolVal==true).count()>0) {
            return true;
        } else {
            return false;
        }
    }


    private Boolean store(StorageIntent storageIntent) {
        storageIntent.nodeFuture
                .thenApply(node -> node.store(storageIntent.key, storageIntent.value));
        return false;
    }

    public static class StorageIntent {
        public CompletableFuture<Node> nodeFuture;
        public String key;
        public String value;

        public StorageIntent(CompletableFuture<Node> nodeFuture, String key, String value) {
            this.nodeFuture = nodeFuture;
            this.key = key;
            this.value = value;
        }
    }
}
