#ifndef MAINWINDOW_H
#define MAINWINDOW_H
#include <QMainWindow>
#include "cgeofile.h"
#include "csearch.h"

namespace Ui {
class MainWindow;
}

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    explicit MainWindow(QWidget *parent = 0);
    ~MainWindow();
    CGeoFile file;
    CSearch search;
    CIndex index;
private slots:
    void on_pushButton_clicked();
    void mousePressEvent(QMouseEvent *event);
private:
    Ui::MainWindow *ui;
};

#endif // MAINWINDOW_H
