#ifndef CPROJECT_H
#define CPROJECT_H


class CProject
{
public:
    CProject();

    int scalex;
    int scaley;

    int ox;
    int oy;
    float wx;
    float wy;
    int uix;
    int uiy;
    void towindows(int x,int y);
    void toworld(float x,float y);
    void toUI(int x,int y);
    void uitoworld(float x,float y);
};

#endif // CPROJECT_H
