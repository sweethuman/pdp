package tech.sweethuman.domain;

import java.text.MessageFormat;
import java.util.LinkedList;
import java.util.Queue;
import java.util.concurrent.locks.Condition;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

public class Buffer {
    private Queue<Integer> queue = new LinkedList<>();
    private Lock lock = new ReentrantLock();
    private Condition condition = lock.newCondition();

    public void put(Integer product) {
        lock.lock();
        try {
            queue.add(product);
            System.out.println(MessageFormat.format("{0} added to buffer", product));
            condition.signal();
        } finally {
            lock.unlock();
        }
    }

    public Integer get() {
        lock.lock();
        try {
            if (queue.isEmpty()) {
                condition.await();
            }
            var value = queue.poll();
            System.out.println(MessageFormat.format("{0} retrieved from buffer", value));
            return value;
        } catch (InterruptedException e) {
            e.printStackTrace();
        } finally {
            lock.unlock();
        }
        return null;
    }
}
