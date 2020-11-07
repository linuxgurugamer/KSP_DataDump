using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// from: https://www.geometrictools.com/Documentation/PolyhedralMassProperties.pdf


namespace KSP_DataDump
{
    class VolumeCalc
    {
        /*
Subexpressions(w0,w1,w2,f1,f2,f3,g0,g1,g2)
{
    temp0=w0+w1;f1=temp0+w2;temp1=w0*w0;temp2=temp1+w1*temp0;
    f2=temp2+w2*f1;f3=w0*temp1+w1*temp2+w2*f2;
    g0=f2+w0*(f1+w0);g1=f2+w1*(f1+w1);g2=f2+w2*(f1+w2);
}
voidCompute(Pointp[],inttmax,intindex[],Real&mass,Point&cm,Matrix&inertia)
{
    constantRealmult[10]={1/6,1/24,1/24,1/24,1/60,1/60,1/60,1/120,1/120,1/120};
    Realintg[10]={0,0,0,0,0,0,0,0,0,0};//order:1,x,y,z,xˆ2,yˆ2,zˆ2,xy,yz,zx
    for(t=0;t<tmax;t++)
    {
        //getverticesoftrianglet
        i0=index[3*t];i1=index[3*t+1];i2=index[3*t+2];
        x0=p[i0].x;y0=p[i0].y;z0=p[i0].z;
        x1=p[i1].x;y1=p[i1].y;z1=p[i1].z;
        x2=p[i2].x;y2=p[i2].y;z2=p[i2].z;
        //getedgesandcrossproductofedges
        a1=x1-x0;b1=y1-y0;c1=z1-z0;a2=x2-x0;b2=y2-y0;c2=z2-z0;
        d0=b1*c2-b2*c1;d1=a2*c1-a1*c2;d2=a1*b2-a2*b1;
        //computeintegralterms
        Subexpressions(x0,x1,x2,f1x,f2x,f3x,g0x,g1x,g2x);
        Subexpressions(y0,y1,y2,f1y,f2y,f3y,g0y,g1y,g2y);
        Subexpressions(z0,z1,z2,f1z,f2z,f3z,g0z,g1z,g2z);
        //updateintegrals
        intg[0]+=d0*f1x;
        intg[1]+=d0*f2x;intg[2]+=d1*f2y;intg[3]+=d2*f2z;
        intg[4]+=d0*f3x;intg[5]+=d1*f3y;intg[6]+=d2*f3z;
        intg[7]+=d0*(y0*g0x+y1*g1x+y2*g2x);
        intg[8]+=d1*(z0*g0y+z1*g1y+z2*g2y);
        intg[9]+=d2*(x0*g0z+x1*g1z+x2*g2z);
    }
    for(i=0;i<10;i++)
    intg[i]*=mult[i];
    mass=intg[0];
    //centerofmass
    cm.x=intg[1]/mass;
    cm.y=intg[2]/mass;
    cm.z=intg[3]/mass;
    //inertiatensorrelativetocenterofmass
    inertia.xx=intg[5]+intg[6]-mass*(cm.y*cm.y+cm.z*cm.z);
    inertia.yy=intg[4]+intg[6]-mass*(cm.z*cm.z+cm.x*cm.x);
    inertia.zz=intg[4]+intg[5]-mass*(cm.x*cm.x+cm.y*cm.y);
    inertia.xy=-(intg[7]-mass*cm.x*cm.y);
    inertia.yz=-(intg[8]-mass*cm.y*cm.z);
    inertia.xz=-(intg[9]-mass*cm.z*cm.x);
}


         */
    }
}
