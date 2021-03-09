#ifndef CPROJECT_H
#define CPROJECT_H


class CProject
{
public:
    CProject();

    double scalex;
    double scaley;
    double xmin;
    double ymin;
    double xmax;
    double ymax;

    double ox;
    double oy;
    double wx;
    double wy;
    double uix;
    double uiy;
    void towindows(double x,double y);
    void toworld(double x, double y);
    void toUI(int x,int y);
    void uitoworld(float x,float y);
};

#endif // CPROJECT_H
