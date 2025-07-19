import os
import numpy as np
from PIL import Image

mid_width = 24
white = np.array([1., 1., 1.])
c = np.array([0x67/255, 0xb6/255, 0xa6/255]) # 679cb6

arr = np.ones((1, 256, 3), float)

for i in range(128 - mid_width // 2):
    k = i / (127 - mid_width // 2)
    arr[0, i] = c + (1 - 0.4 * k**2) * (white - c)

for i in range(128):
    arr[0, 255 - i] = arr[0, i]

arr = np.clip(arr * 256, 0, 255).astype(np.uint8)
img_out = Image.fromarray(arr, "RGB")
img_out.save(os.path.join(os.path.dirname(__file__), "hold_texture.png"))
