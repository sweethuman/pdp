package tech.sweethuman.domain;

import java.time.LocalDateTime;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

public class Account {
    public int uid;
    public int balance;
    public Log log;
    public int initialBalance;
    public Lock mtx;

    public Account(int uid, int balance) {
        this.uid = uid;
        this.initialBalance = balance;
        this.balance = balance;
        this.mtx = new ReentrantLock();
        this.log = new Log();
    }

    public String makeTransfer(Account receiver, int sum) {
        if (uid == receiver.uid) {
            return "The accounts are the same!";
        }
        if (this.uid < receiver.uid) {
            this.mtx.lock();
            receiver.mtx.lock();
        } else {
            receiver.mtx.lock();
            this.mtx.lock();
        }

        if (sum > balance) {
            if (this.uid < receiver.uid) {
                receiver.mtx.unlock();
                this.mtx.unlock();
            } else {
                this.mtx.unlock();
                receiver.mtx.unlock();
            }
            return "Not a valid transfer!";
        }

        balance -= sum;
        receiver.balance += sum;
        var operation = new Operation(uid, receiver.uid, sum, LocalDateTime.now());
        log.addToLog(operation);
        receiver.log.addToLog(operation);

        if (this.uid < receiver.uid) {
            receiver.mtx.unlock();
            this.mtx.unlock();
        } else {
            this.mtx.unlock();
            receiver.mtx.unlock();
        }
        return operation.toString();
    }

    public boolean check() {
        int initBalance = this.initialBalance;
        for (Operation operation : this.log.operations) {
            if (operation.src == uid) {
                initBalance -= operation.amount;
            } else {
                initBalance += operation.amount;
            }
        }
        return initBalance == this.balance;
    }
}
