package tech.sweethuman.domain;

import java.time.LocalDateTime;
import java.time.temporal.ChronoUnit;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import java.util.concurrent.atomic.AtomicInteger;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

public class Bank {
    public List<Account> accounts;
    private static final int AMOUNT_THREADS = 10;
    private static final int AMOUNT_ACCOUNTS = 100;
    private static final int AMOUNT_OPERATION = 1000;
    private static final int INIT_BALANCE = 500;

    private Lock mtx = new ReentrantLock();
    private boolean check = false;


    public Bank() {
        accounts = new ArrayList<>();
    }

    public void threadWork(int threadId) {
        Random r = new Random();
        for (long operation = 0; operation < AMOUNT_OPERATION; ++operation) {
            int accId = r.nextInt(AMOUNT_ACCOUNTS);
            int accId2 = r.nextInt(AMOUNT_ACCOUNTS);
            if (accId == accId2) {
                --operation;
                continue;
            }

            int sum = r.nextInt(INIT_BALANCE / 2);
            String operationOutput = accounts.get(accId).makeTransfer(accounts.get(accId2), sum);
            System.out.println("[Thread " + threadId + "]:" + operationOutput);
        }
    }

    public void initAccounts() {
        int uid = 0;
        accounts = new ArrayList<>();
        for (int i = 0; i < AMOUNT_ACCOUNTS; ++i) {
            accounts.add(new Account(uid++, 200));
        }
    }

    public void run() {
        initAccounts();
        var start = LocalDateTime.now();
        List<Thread> threads = new ArrayList<>();
        for (int i = 0; i < AMOUNT_THREADS; i++) {
            int finalI = i;
            threads.add(new Thread(() -> {
                this.threadWork(finalI);
            }));
        }

        threads.forEach(Thread::start);
        check = true;
        Thread checker = new Thread(() -> {
            mtx.lock();
            while (check) {
                mtx.unlock();
                Random r = new Random();
                if (r.nextInt(9) == 0) {
                    runCorrectnessCheck();
                }
                mtx.lock();
            }
            mtx.unlock();
        });

        checker.start();
        threads.forEach(thread -> {
            try {
                thread.join();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        });
        mtx.lock();
        check = false;
        mtx.unlock();

        try {
            checker.join();
        } catch (InterruptedException e) {
            e.printStackTrace();
        }

        runCorrectnessCheck();

        var end = LocalDateTime.now();
        var seconds = ChronoUnit.SECONDS.between(start, end);
        System.out.println("Time spent: " + (seconds) + " seconds");
    }

    private void runCorrectnessCheck() {
        System.out.println("Running correctness check");
        AtomicInteger failedAccounts = new AtomicInteger();
        accounts.forEach(account -> {
            account.mtx.lock();
            if (!account.check()) {
                failedAccounts.getAndIncrement();
            }
            account.mtx.unlock();
        });

        for (Account account : accounts) {
            account.mtx.lock();
            for (Operation op : account.log.operations) {
                Account targetAccount = accounts.get(op.dest);
                if (!targetAccount.log.operations.contains(op)) {
                    failedAccounts.getAndIncrement();
                }
            }
            account.mtx.unlock();
        }

        if (failedAccounts.get() > 0) {
            throw new RuntimeException("Correctness check failed! " + failedAccounts.get() + " Accounts have failed");
        }
        System.out.println("Finished correctness check");
    }
}
