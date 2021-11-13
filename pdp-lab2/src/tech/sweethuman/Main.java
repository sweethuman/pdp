package tech.sweethuman;

import tech.sweethuman.domain.Buffer;
import tech.sweethuman.domain.ConsumerThread;
import tech.sweethuman.domain.ProducerThread;

import java.util.ArrayList;
import java.util.Arrays;

public class Main {

    public static void main(String[] args) throws InterruptedException {
        var v1 = new ArrayList<Integer>(Arrays.asList(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14));
        var v2 = new ArrayList<Integer>(Arrays.asList(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14));
        var buffer = new Buffer();
        var producer = new ProducerThread(buffer, v1, v2);
        var consumer = new ConsumerThread(buffer, v1.size());

        consumer.start();
        producer.start();
        producer.join();
        consumer.join();
    }
}
