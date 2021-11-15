package tech.sweethuman.model;


import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

public abstract class MatrixMultiplication {
    protected int size;
    protected int[][] a;
    protected int[][] b;
    protected int[][] result;
    protected int step_i;
    protected int numberOfTasks;
    protected Lock lock = new ReentrantLock();

    public MatrixMultiplication(int[][] a, int[][] b, int size, int numberOfTasks) {
        this.a = a;
        this.b = b;
        this.size = size;
        this.result = new int[size][size];
        this.step_i = 0;
        this.numberOfTasks = numberOfTasks;
    }

    protected void multiplySubTask(int row_a_index, int col_b_index) {
        // computes a single element of the resulting matrix
        for (int i = 0; i < size; i++) {
            result[row_a_index][col_b_index] += a[row_a_index][i] * b[i][col_b_index];
        }
    }

    public abstract Runnable getRunnable();

    public int[][] getResult() {
        return result;
    }

    public int getNrOfTasks() {
        return numberOfTasks;
    }
}
