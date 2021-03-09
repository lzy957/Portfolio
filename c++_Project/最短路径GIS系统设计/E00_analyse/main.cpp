#include <iostream>
#include <QApplication>
#include "mainwindow.h"
using namespace std;
#include "cfile.h"

int main(int argc,char* argv[])
//int main()
{

//    CFile file;
//    file.FileOpen("/Users/apple/Downloads/GIS_design/rivercopy.e00");
    //OpenGL图形绘制

    QApplication app(argc,argv);
    MainWindow w;
    w.show();

    cout << "Hello World!" << endl;

    return app.exec();
//    return 0;
}
