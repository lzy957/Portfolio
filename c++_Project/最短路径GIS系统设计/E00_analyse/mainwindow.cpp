#include "mainwindow.h"
#include "ui_mainwindow.h"
#include <QTextCodec>

MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow)
{
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
    ui->openGLWidget->file->search->clear();
    ui->label->setText("wait...");
    QString py,py1;
    py=ui->lineEdit->text();
    py1=ui->lineEdit_2->text();
    int numf=py.toInt();
    int numt=py1.toInt();
    ui->openGLWidget->file->search->endnum=numt;
    ui->openGLWidget->file->search->fromnum=numf;
    ui->openGLWidget->file->search->shortpath(numf,ui->openGLWidget->file->E00);
//    bool result=false;
//    result=search.findtonode(numf,numt,file.E00);
//    if(result)
//        ui->label->setText("accesible!");
//    else
//        ui->label->setText("failed!");
    ui->openGLWidget->file->search->path[0]=numf;
    ui->openGLWidget->file->search->path[ui->openGLWidget->file->search->countg]=numt;
    QString numtempt;
    QString patht="";
    for(int i=0;i<ui->openGLWidget->file->search->countg;i++)
    {
//        numtempt=search.path[i];
        numtempt=tr("%1").arg(ui->openGLWidget->file->search->path[i]);
        patht=patht+numtempt+"to";
    }
    numtempt=tr("%1").arg(ui->openGLWidget->file->search->path[ui->openGLWidget->file->search->countg]);
    patht=patht+numtempt;
    numtempt=tr("%1").arg(ui->openGLWidget->file->search->length);
    patht=patht+"  "+numtempt;
    ui->label->setText(patht);
    ui->openGLWidget->file->search->SetList(ui->openGLWidget->file->E00);
    ui->openGLWidget->click=true;
    ui->openGLWidget->file->search->Draw(ui->openGLWidget->file->prj);
    ui->openGLWidget->update();
    //名称检索
//    CChncity* tempt=new CChncity;
//    tempt=index.SearchNameIndex(py,file.Cityset,file.map->proj);
    //字符编码
//    if(tempt!=0x00)
//    {
//        while(!tempt->isutf8)
//        {
//            gb2312ToUtf8(tempt->name);
//            tempt->isutf8=true;
//        }
//        //输出
//        QString xs,ys,nameqs;
//        nameqs=QString::fromStdString(tempt->name);
//        xs=tr( " coordination x: %1" ).arg(tempt->x);
//        ys=tr( " coordination y: %1" ).arg(tempt->y);
//        ui->label->setText("name:"+nameqs+xs+ys);
    if(ui->openGLWidget->file->search->countg!=0)
    {
        CPoint *tempt=new CPoint;
        tempt->setPoint((*ui->openGLWidget->file->search->pathlist.begin())->fnode->pt);
            ui->openGLWidget->file->prj->toUI(tempt->x,tempt->y);
            ui->label_2->setGeometry(ui->openGLWidget->file->prj->uix-7,ui->openGLWidget->file->prj->uiy-17,15,15);
            ui->label_2->setText("⬇️");
    //        list<CArc*> ::iterator i;
    //        i=ui->openGLWidget->file->search->pathlist.back();
            ui->openGLWidget->file->prj->toUI(ui->openGLWidget->file->search->pathlist.back()->tnode->pt.x,ui->openGLWidget->file->search->pathlist.back()->tnode->pt.y);
            ui->label_4->setGeometry(ui->openGLWidget->file->prj->uix-7,ui->openGLWidget->file->prj->uiy-17,15,15);
            ui->label_4->setText("🏁");
    }
    else
        ui->label->setText("no path");

//    }
//    else
//        {
//            ui->label->setText("no record");

//        }
}

void MainWindow::mousePressEvent(QMouseEvent *event)
{
    //屏幕点获
    /*float xt,yt;
    float x=event->pos().x();
    float y=event->pos().y();
    xt=0-x;
    yt=0-y;
    glTranslated(xt,yt,0);
    glScaled(2,2,0);*/
    //坐标转换
//    file.map->proj->uitoworld(x,y);
//    CGeopoint pt;
//    pt.x=file.map->proj->ox;
//    pt.y=file.map->proj->oy;
    //四叉树搜索
//    CChncity* tempt=new CChncity;
//    tempt=search.GraphicsSearch(file.map->wrect,pt,file.Cityset);
    //字符编码
//    if(tempt!=0x00)
//    {
//        while(!tempt->isutf8)
//        {
//            gb2312ToUtf8(tempt->name);
//            tempt->isutf8=true;
//        }
//        //输出
//        QString xs,ys,nameqs;
//        nameqs=QString::fromStdString(tempt->name);
//        xs=tr( " coordination x: %1" ).arg(tempt->x);
//        ys=tr( " coordination y: %1" ).arg(tempt->y);
//        ui->label->setText("name:"+nameqs+xs+ys);
//        file.map->proj->toUI(tempt->x,tempt->y);
//        ui->label_2->setGeometry(file.map->proj->uix-6,file.map->proj->uiy-17,15,15);
//        ui->label_2->setText("⬇️");
//    }
//else
//        ui->label->setText("Try again");

}
