#include "cmyopenglwidgt.h"
#include <iostream>
#include "ui_mainwindow.h"

using namespace std;
CMyOpenGLWidgt::CMyOpenGLWidgt(QWidget *parent)
    :QOpenGLWidget(parent)
{
}

void CMyOpenGLWidgt::initializeGL()
{
    //为当前环境初始化OpeGL函数
    initializeOpenGLFunctions();
    glClearColor(1.0,1.0,1.0,1.0);
    glShadeModel(GL_SMOOTH);
//    glEnable(GL_DEPTH);
    //创建顶点着色器
//    QOpenGLShader* vshader= new QOpenGLShader(QOpenGLShader::Vertex,this);
//    const char* vsrc=
//            "in vec4 vPosition;                   \n"
//            "void main(){                         \n"
//            "   gl_Position=vPosition;\n"
//            "}                                    \n";
//    vshader->compileSourceCode(vsrc);
//    //创建片段着色器
//    QOpenGLShader* fshader= new QOpenGLShader(QOpenGLShader::Fragment,this);
//    const char* fsrc=
//            "void main(){                          \n"
//            "   gl_FragColor=vec4(0.5,0.5,0.0,1.0);\n"
//            "}                                     \n";
//    fshader->compileSourceCode(fsrc);
// //    创建着色器程序
//    program=new QOpenGLShaderProgram;
//    program->addShader(vshader);
//    program->addShader(fshader);
//    program->link();
//    program->bind();
    //在initializeGL函数中添加opengl版本检查
//    cout<<glGetString(GL_VERSION)<<endl;

//     glClearColor(0.0,0.0,0.0);

}

void CMyOpenGLWidgt::paintGL()
{
//    GLfloat vertices[]={
//        -0.8f,0.8f
//        -0.8f,-0.8f,
//        0.8f,-0.8f,
//        0.8f,0.8f
//    };
//    GLuint vPosition=program->attributeLocation("vPosition");
//    glVertexAttribPointer(vPosition,2,GL_FLOAT,GL_FALSE,0,vertices);
//    glEnableVertexAttribArray(vPosition);
//    glDrawArrays(GL_TRIANGLE_FAN,0,4);
    file.FileOpen();
    glClear(GL_COLOR_BUFFER_BIT|GL_DEPTH_BUFFER_BIT);
//    int w=file.map->wrect.right-file.map->wrect.left;
//    int h=file.map->wrect.top-file.map->wrect.bottom;
//    int w=width();
//    int h=height();
//    int side =qMin(w,h);
//    glViewport((w-side)/2,(h-side)/2,w,h);

    glMatrixMode(GL_MODELVIEW);
//    glMatrixMode(GL_PROJECTION);
    glLoadIdentity();
//    glOrtho(file.map->wrect.left/1000000,file.map->wrect.right/1000000,file.map->wrect.bottom/1000000,file.map->wrect.top/1000000,-10,10);
//    gluLookAt((right-left)/2+left,(top-bottom)/2+bottom,20,(right-left)/2+left,(top-bottom)/2+bottom,0,1,1,1);
    file.map->Draw();
    file.Cityset.Draw(file.map->proj);

//    glPointSize(10);
//    glBegin(GL_POINTS);
//    glColor3f(0.0,0.0,0.0);
//    glVertex2f(0,0);
//    glEnd();
//    glEnable(GL_POINT_SMOOTH);
//    glEnable(GL_BLEND);
//    glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
//    glFlush();
//    glOrtho(-1, 3, -1, 3, -1, 1);
}

void CMyOpenGLWidgt::resizeGL(int width, int height)
{
//        int side =qMin(width,height);
//        glViewport((width-side)/2,(height-side)/2,side,side);
}


