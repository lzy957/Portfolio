#include "mainwindow.h"
#include "ui_mainwindow.h"
#include <QTextCodec>

MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow)
{
    file.FileOpen();
    search.Quadtree.QuadtreeBuild(file.Cityset,file.map->wrect);
    index.NameIndexset(file.Cityset);
    ui->setupUi(this);
}

MainWindow::~MainWindow()
{
    delete ui;
}

void gb2312ToUtf8(std::string& strGb2312)
{

QTextCodec* utf8Codec= QTextCodec::codecForName("utf-8");
QTextCodec* gb2312Codec = QTextCodec::codecForName("gb2312");

QString strUnicode= gb2312Codec->toUnicode(strGb2312.c_str());
QByteArray ByteUtf8= utf8Codec->fromUnicode(strUnicode);

strGb2312= ByteUtf8.data();
}



void MainWindow::on_pushButton_clicked()
{
    //输入获取
    QString py;
    py=ui->lineEdit->text();
    //名称检索
    CChncity* tempt=new CChncity;
    tempt=index.SearchNameIndex(py,file.Cityset,file.map->proj);
    //字符编码
    if(tempt!=0x00)
    {
        while(!tempt->isutf8)
        {
            gb2312ToUtf8(tempt->name);
            tempt->isutf8=true;
        }
        //输出
        QString xs,ys,nameqs;
        nameqs=QString::fromStdString(tempt->name);
        xs=tr( " coordination x: %1" ).arg(tempt->x);
        ys=tr( " coordination y: %1" ).arg(tempt->y);
        ui->label->setText("name:"+nameqs+xs+ys);
        file.map->proj->toUI(tempt->x,tempt->y);
        ui->label_2->setGeometry(file.map->proj->uix-6,file.map->proj->uiy-17,15,15);
        ui->label_2->setText("⬇️");
    }
    else
        {
            ui->label->setText("no record");

        }
}

void MainWindow::mousePressEvent(QMouseEvent *event)
{
    //屏幕点获取
    float x=event->pos().x();
    float y=event->pos().y();
    //坐标转换
    file.map->proj->uitoworld(x,y);
    CGeopoint pt;
    pt.x=file.map->proj->ox;
    pt.y=file.map->proj->oy;
    //四叉树搜索
    CChncity* tempt=new CChncity;
    tempt=search.GraphicsSearch(file.map->wrect,pt,file.Cityset);
    //字符编码
    if(tempt!=0x00)
    {
        while(!tempt->isutf8)
        {
            gb2312ToUtf8(tempt->name);
            tempt->isutf8=true;
        }
        //输出
        QString xs,ys,nameqs;
        nameqs=QString::fromStdString(tempt->name);
        xs=tr( " coordination x: %1" ).arg(tempt->x);
        ys=tr( " coordination y: %1" ).arg(tempt->y);
        ui->label->setText("name:"+nameqs+xs+ys);
        file.map->proj->toUI(tempt->x,tempt->y);
        ui->label_2->setGeometry(file.map->proj->uix-6,file.map->proj->uiy-17,15,15);
        ui->label_2->setText("⬇️");
    }
else
        ui->label->setText("Try again");

}
