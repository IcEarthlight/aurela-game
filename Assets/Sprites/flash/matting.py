import os
import colorsys
import subprocess

import numpy as np
from PIL import Image

output_filename = "flash_%03d.png"

def mat_img(filename):
    img = Image.open(os.path.join(os.path.dirname(__file__), filename)).convert("RGBA")
    width, height = img.size
    img = img.crop((1, 1, width - 1, height - 1))
    # img_rotated = img.rotate(90, expand=True)
    width, height = img.size
    
    img_resized = img.resize((3*width//5, 3*height//5), Image.Resampling.LANCZOS)
    arr = np.array(img_resized, dtype=np.float32) / 255.0

    for i, j in np.ndindex(arr.shape[:2]):
        r, g, b, a = arr[i, j]
        h, s, v = colorsys.rgb_to_hsv(r, g, b)
        a = (r + g + b) / 3
        r, g, b = colorsys.hsv_to_rgb(h + 0.3, 0.8 * s, 1)
        arr[i, j] = [r, g, b, a]

    arr = np.clip(arr * 256, 0, 255).astype(np.uint8)
    img_out = Image.fromarray(arr, "RGBA")
    img_out.save(os.path.join(os.path.dirname(__file__), filename))

for filename in os.listdir(os.path.dirname(__file__)):
    if filename.endswith(".gif"):
        subprocess.run([
            "ffmpeg", "-i",
            os.path.join(os.path.dirname(__file__), filename),
            os.path.join(os.path.dirname(__file__), output_filename)
        ])

i = 1
while os.path.exists(os.path.join(os.path.dirname(__file__), output_filename % i)):
    mat_img(output_filename % i)
    i += 1
