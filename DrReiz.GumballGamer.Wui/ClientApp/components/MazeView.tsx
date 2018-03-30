import * as React from 'react';
import { RouteComponentProps } from 'react-router';

interface MazeState {
    currentCount: number;
}

export class MazeView extends React.Component<RouteComponentProps<{}>, MazeState> {
    constructor() {
        super();
        this.state = { currentCount: 0 };
    }

    public render() {
        let resizeK = 0.5;
        let maze = MazeView.Resize(MazeView.Maze, resizeK);
        let grid = MazeView.ResizeRects(MazeView.Grid(MazeView.Maze.frame.width, MazeView.Maze.frame.height, 200), resizeK);

        return <div>
            <h1>Maze</h1>
            <div style={{ display: 'table-row' }}>
                <div style={{ width: maze.frame.width, height: maze.frame.height, border: '0px solid black', display: 'table-cell', position: 'relative' }}>
                    <img style={{ width: maze.frame.width, height: maze.frame.height, position: 'absolute' }} src='data/171213.233335.png' / >
                    {grid.map(area =>
                        <div style={{ left: area.x, top: area.y, width: area.width, height: area.height, position: 'absolute', border:'1px solid lightblue' }}>
                        </div>
                       )
                    }
                    {maze.areas.map(area =>
                        <div style={{ left: area.x, top: area.y, width: area.width, height: area.height, position: 'absolute', border:'1px solid red' }}>
                        </div>
                       )
                    }
                </div>
                <div style={{ width: '20px', display: 'table-cell' }}></div>
                <div style={{ width: maze.frame.width, height: maze.frame.height, border: '1px solid black', display: 'table-cell', position:'relative' }}>
                    {maze.areas.map(area =>
                        <div style={{ left: area.x, top: area.y, width: area.width, height: area.height, position: 'absolute', border:'1px solid black' }}>
                        </div>
                       )
                    }
                </div>
            </div>

        </div>;
    }
    static Grid(width: number, height: number, size: number): Rect[] {
        return [...Array(Math.floor((height - 1) / size - 1)).keys()].map(j => { return { x: 0, y: (j + 1) * size, width: width, height: size }; })
            .concat(
            [...Array(Math.floor((width - 1) / size - 1)).keys()].map(i => { return { x: (i + 1) * size, y: 0, width: size, height: height }; })
                );
    }
    static Resize(maze: Maze, k:number): Maze {
        return {
            frame: {
                x: k * maze.frame.x,
                y: k * maze.frame.y,
                width: k * maze.frame.width,
                height: k * maze.frame.height
            },
            areas: maze.areas.map(area => {
                return {
                    x: k * area.x,
                    y: k * area.y,
                    width: k * area.width,
                    height: k * area.height,
                    name: area.name,
                    value: area.value
                };
            })
        }
    }
    static ResizeRects(rects: Rect[], k:number): Rect[] {
        return rects.map(rect => {
            return {
                x: k * rect.x,
                y: k * rect.y,
                width: k * rect.width,
                height: k * rect.height,
            };
        })
    }

    //incrementCounter() {
    //    this.setState({
    //        currentCount: this.state.currentCount + 1
    //    });
    //}
    static Maze: Maze = {
        frame: {
            x: 0, y: 0, width: 900, height: 1600
        },
        areas: [
            { x: 30, y: 280, width: 170, height: 156 },
            { x: 200, y: 436, width: 170, height: 156 },
            { x: 370, y: 592, width: 170, height: 156 },
            { x: 540, y: 748, width: 170, height: 156 },
            { x: 710, y: 904, width: 170, height: 156 },
            { x: 540, y: 1060, width: 170, height: 156 },
            ]
    };
}

interface Rect {
    x: number;
    y: number;
    width: number;
    height: number;
}

interface Maze {
    frame: Rect;
    areas: Area[];
}
interface Area extends Rect {
    name?: string;
    value?: string;
}
