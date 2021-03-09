#ifndef CTOL_H
#define CTOL_H

typedef struct tolstruct
{
    int num;
    int isverified;
    double tol;
}tolgroup;

class CTOL
{
public:
    CTOL();
    int precision;
    tolgroup tol[10];
};

#endif // CTOL_H
