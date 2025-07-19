import os

import numpy as np
from PIL import Image

input_filename = "raw.png"
output_filename = "out.png"
gradient_filename = [
    "/home/ethlt/Downloads/gradient6.png",
    "/home/ethlt/Downloads/gradient7_2.png",
    "/home/ethlt/Downloads/gradient8.png",
    "/home/ethlt/Downloads/gradient9.png",
    "/home/ethlt/Downloads/gradient10.png"
]

img = Image.open(os.path.join(os.path.dirname(__file__), input_filename)).convert("RGBA")
arr = np.array(img, dtype=np.float32)
arr[:,:,0] = (arr[:,:,0] - 38) / (255 - 38)
arr[:,:,1] = (arr[:,:,1] - 51) / (255 - 51)
arr[:,:,2] = (arr[:,:,2] - 45) / (255 - 45)


# print(arr[:,:,0].min(), arr[:,:,1].min(), arr[:,:,2].min(), arr[:,:,3].min())
# print(arr[:,:,0].max(), arr[:,:,1].max(), arr[:,:,2].max(), arr[:,:,3].max())

arr[:,:,3] = np.average(arr[:,:,:3], axis=2)
arr[:,:,:3] = 1
# for i, j in np.ndindex(arr.shape[:2]):
#     arr[i, j, :3] = gradarr[int(i / arr.shape[0] * gradarr.shape[0]), int(j / arr.shape[1] * gradarr.shape[1])]

split_y = [0, 212, 338, 453, 565, arr.shape[0]]

for i in range(len(split_y) - 1):
    grad = Image.open(os.path.join(os.path.dirname(__file__), gradient_filename[i])).convert("RGB")
    gradarr = np.array(grad, dtype=np.float32)
    gradarr /= 255
    grad.close()

    part_arr = arr[split_y[i]:split_y[i+1], :, :]

    non_transparent_indices = np.where(part_arr[:, :, 3] > 0)

    if non_transparent_indices[0].size > 0:
        min_y, max_y = non_transparent_indices[0].min(), non_transparent_indices[0].max()
        min_x, max_x = non_transparent_indices[1].min(), non_transparent_indices[1].max()
        cropped_part_arr = part_arr[min_y:max_y+1, min_x:max_x+1, :]
    else:
        cropped_part_arr = part_arr
    
    for j, k in np.ndindex(cropped_part_arr.shape[:2]):
        cropped_part_arr[j, k, :3] = gradarr[int(j / cropped_part_arr.shape[0] * gradarr.shape[0]), int(k / cropped_part_arr.shape[1] * gradarr.shape[1])]
    
    cropped_part_arr = np.clip(cropped_part_arr * 256, 0, 255).astype(np.uint8)
    cropped_img = Image.fromarray(cropped_part_arr, "RGBA")

    output_part_filename = os.path.join(os.path.dirname(__file__), f"output_part_{i+1}.png")
    cropped_img.save(output_part_filename)
    print(f"Saved {output_part_filename}")

arr = np.clip(arr * 256, 0, 255).astype(np.uint8)
img_out = Image.fromarray(arr, "RGBA")
img_out.save(os.path.join(os.path.dirname(__file__), output_filename))
