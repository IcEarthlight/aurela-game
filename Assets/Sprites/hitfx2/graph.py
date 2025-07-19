import numpy as np
import matplotlib.pyplot as plt

# plt.axis([0, 1, 0, 1])

x = np.linspace(0, 1, 65)
# y = 2 ** (20 * (x-1))
# y = 4 / np.pi * np.sin(x*np.pi / 2)
# plt.plot(x, y)
# for i in range(3, 20, 2):
#     y += 4 / np.pi * np.sin(x*np.pi / 2 * i) / i
#     plt.plot(x, y, alpha=i/20)

x_ = np.linspace(0, 1, 17)
y_ = np.array([
    0, 0, 0, 0, 0,
    1/12800,
    2/12800,
    5/12800,
    13/12800,
    30/12800,
    72/12800,
    171/12800,
    405/12800,
    960/12800,
    2276/12800,
    5389/12800,
    1
])

y = np.sqrt(np.sqrt(1-(x-1)**2))
y_ = np.sqrt(np.sqrt(1-(x-1)**2))*3-2
y__ = (1-(x-1)**2)*4/5
y___ = (1-(x-1)**2)*4/5-(-x/2+0.5)**2
plt.plot(x, y)
plt.plot(x, y_)
plt.plot(x, y__)
plt.plot(x, y___)
plt.axis([0, 1, 0, 1])
plt.show()