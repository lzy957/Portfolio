#include "cmyopenglwidgt.h"
#include <iostream>
#include "ui_mainwindow.h"
#include <openGL/glu.h>

using namespace std;
CMyOpenGLWidgt::CMyOpenGLWidgt(QWidget *parent)
    :QOpenGLWidget(parent)
{
    file=new CFile;
    click=false;
}

void CMyOpenGLWidgt::initializeGL()
{
    //为当前环境初始化OpeGL函数
    initializeOpenGLFunctions();
    glClearColor(1.0,1.0,1.0,1.0);
    glShadeModel(GL_SMOOTH);
    m_iMag = 1;

}

void CMyOpenGLWidgt::paintGL()
{

    file->FileOpen("/Users/apple/Downloads/GIS_design/whu.e00");
    glClear(GL_COLOR_BUFFER_BIT|GL_DEPTH_BUFFER_BIT);

//    glMatrixMode(GL_MODELVIEW);
//    glMatrixMode(GL_PROJECTION);
//    glLoadIdentity();
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
    glMatrixMode(GL_PROJECTION);
    glLoadIdentity();
//    gluPerspective(10.0 * m_iMag, m_iWidth / m_iHeight, -8.0, 8.0);
    glMatrixMode(GL_MODELVIEW);
    glLoadIdentity();
//    gluLookAt(3100 , 4200, 1.0, 10, 41, 0.0, 0.0, 1.0, 0.0);
//    glOrtho(file.map->wrect.left/1000000,file.map->wrect.right/1000000,file.map->wrect.bottom/1000000,file.map->wrect.top/1000000,-10,10);
//    gluLookAt((right-left)/2+left,(top-bottom)/2+bottom,20,(right-left)/2+left,(top-bottom)/2+bottom,0,1,1,1);

    glBegin(GL_QUADS);
    file->E00->Draw(file->prj);
//    file.Cityset.Draw(file.map->proj);
    if(click)
    {
        file->search->Draw(file->prj);
    }
//    glPointSize(10);
//    glBegin(GL_POINTS);
//    glColor3f(0.0,0.0,0.0);
//    glVertex2f(0,0);
    glEnd();
//    click=false;
//    glEnable(GL_POINT_SMOOTH);
//    glEnable(GL_BLEND);
//    glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
//    glFlush();
//    glOrtho(-1, 3, -1, 3, -1, 1);
}

void CMyOpenGLWidgt::resizeGL(int width, int height)
{
    if(width > 0 && height >0)
    {
        glViewport(0, 0, (GLint)width, (GLint)height);//这句话把显示的范围跟glWindow范围划了等号
        m_iWidth = width;
        m_iHeight = height;
    }
//        int side =qMin(width,height);
//        glViewport((width-side)/2,(height-side)/2,side,side);
}

void CMyOpenGLWidgt::wheelEvent(QWheelEvent *e)
{

   /* QPoint qpMag = e->angleDelta();
    int iMag = qpMag.y();
    bool bUpdate = false;
    if(iMag > 0)
    {
        if(m_iMag < 8)
        {
            m_iMag *= 2;
            bUpdate = true;
        }
    }

    if(iMag < 0)
    {
        if(m_iMag > 1)
        {
            m_iMag /= 2;
            bUpdate = true;
        }
    }

    if(bUpdate)
    {
        update();
    }*/
}

void CMyOpenGLWidgt::mousePressEvent(QMouseEvent *event)
{
//    click=true;
        float xt,yt;
        float x=event->pos().x();
        float y=event->pos().y();
        xt=0-x;
        yt=0-y;
        gluLookAt(x,y,5,x,y,0,0,1,0);
//        glTranslated(xt,yt,0);
        glScaled(2,2,0);
}
