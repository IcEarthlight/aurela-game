import PIL.Image
import PIL.ImageDraw
# import PIL.PngImagePlugin
import numpy as np
import matplotlib.image

print(__import__("os").getcwd())

_f = lambda x : np.sqrt(np.sqrt(1-(x-1)**2))
_g = lambda x : max(0, np.sqrt(np.sqrt(1-(x-1)**2))*3-2)
f_ = lambda x : max(0, min(_g(x), (1-(x-1)**2)*4/5))
g_ = lambda x : max(0, min(_g(x), (1-(x-1)**2)*4/5-(-x/2+0.5)**2))

def rounded_rect(im : PIL.ImageDraw.Draw, k, r, c, depth=1.0) -> PIL.ImageDraw.Draw:
    cx, cy = w*k/2, h*k/2
    r *= k
    c *= k
    if c == 0:
        pointList = [
            (cx + r, cy - r),
            (cx - r, cy - r),
            (cx - r, cy + r),
            (cx + r, cy + r),
        ]
        pointList = [
            (cx + r/2**0.5, cy),
            (cx, cy + r/2**0.5),
            (cx - r/2**0.5, cy),
            (cx, cy - r/2**0.5),
        ]
    else:
        pointList = [
            (cx + r    , cy - r + c),
            (cx + r - c, cy - r    ),
            (cx - r + c, cy - r    ),
            (cx - r    , cy - r + c),
            (cx - r    , cy + r - c),
            (cx - r + c, cy + r    ),
            (cx + r - c, cy + r    ),
            (cx + r    , cy + r - c)
        ]
    im.polygon(
        pointList,
        fill = (255, 255, 255, round(depth*255))
    )
    return im

k = 16
frams = 34

w, h = 256, 32

for i in range(frams):
    maskTexture = PIL.Image.new("RGBA", (w*k, h*k))
    maskTextureDraw = PIL.ImageDraw.Draw(maskTexture)
    maskTextureDraw = rounded_rect(maskTextureDraw, k, _f((i+1)/frams)*w/2, 0)
    maskTextureDraw = rounded_rect(maskTextureDraw, k, _g((i+1)/frams)*w/2, 0, depth=0)
    maskTextureDraw = rounded_rect(maskTextureDraw, k, f_((i+1)/frams)*w/2, 0, depth=0.5)
    maskTextureDraw = rounded_rect(maskTextureDraw, k, g_((i+1)/frams)*w/2, 0, depth=0)
    maskTexture = maskTexture.resize((w, h))

    maskTextureArr = np.array(maskTexture).astype(np.float64)

    maskTexture.save("hitFX_"+"{}.png".format(i).rjust(6, '0'))
