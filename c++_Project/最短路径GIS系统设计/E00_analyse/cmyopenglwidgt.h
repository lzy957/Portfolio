#ifndef CMYOPENGLWIDGT_H
#define CMYOPENGLWIDGT_H
#include <QOpenGLWidget>
#include <QOpenGLFunctions>
#include <QWheelEvent>
#include "cfile.h"
//#include "csearch.h"
class CMyOpenGLWidgt:public QOpenGLWidget,protected QOpenGLFunctions
{
        Q_OBJECT
public:
    explicit CMyOpenGLWidgt(QWidget* parent = 0);
    CFile *file;
    int           m_iWidth;
    int           m_iHeight;
    int           m_iMag;
    bool click;
protected:
    void initializeGL();
    void paintGL();
    void resizeGL(int width,int height);
    virtual void wheelEvent(QWheelEvent *e);
    virtual void mousePressEvent(QMouseEvent *event);
private:

    QOpenGLShaderProgram* program;
};

#endif // CMYOPENGLWIDGT_H
