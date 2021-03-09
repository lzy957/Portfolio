#ifndef CMYOPENGLWIDGT_H
#define CMYOPENGLWIDGT_H
#include <QOpenGLWidget>
#include <QOpenGLFunctions>
#include "cgeofile.h"
#include "csearch.h"
class CMyOpenGLWidgt:public QOpenGLWidget,protected QOpenGLFunctions
{
        Q_OBJECT
public:
    explicit CMyOpenGLWidgt(QWidget* parent = 0);
    CGeoFile file;
protected:
    void initializeGL();
    void paintGL();
    void resizeGL(int width,int height);

private:

    QOpenGLShaderProgram* program;
};

#endif // CMYOPENGLWIDGT_H
