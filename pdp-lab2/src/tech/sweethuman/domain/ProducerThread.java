package tech.sweethuman.domain;

import java.util.List;

public class ProducerThread extends Thread {
    private final Buffer buffer;
    private final List<Integer> vector1;
    private final List<Integer> vector2;

    public ProducerThread(Buffer buffer, List<Integer> vector1, List<Integer> vector2) {
        this.buffer = buffer;
        this.vector1 = vector1;
        this.vector2 = vector2;
    }

    @Override
    public void run() {
        for (var i = 0; i < vector1.size(); i++) {
            try {
                if (i % 2 == 0) sleep(250);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
            var product = vector1.get(i) * vector2.get(i);
            buffer.put(product);
        }
        try {
            sleep(250);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
        System.out.println("Finished producing!");
    }
}
