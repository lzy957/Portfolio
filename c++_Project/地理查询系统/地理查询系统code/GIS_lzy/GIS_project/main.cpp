#include <iostream>
#include <QApplication>
#include "mainwindow.h"
using namespace std;

int main(int argc,char* argv[])
{

    //OpenGL图形绘制

    QApplication app(argc,argv);
    MainWindow w;
    w.show();

    cout << "Hello World!" << endl;

    return app.exec();
}
