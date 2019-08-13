import React, { Component } from 'react';
import { Button, Spinner, Input } from 'reactstrap';
import axios from 'axios'
import { optional } from 'optional-chain'

interface MazeState {
  game: string;
  is_grid_display: boolean;
  selectedVisionShot: VisionShot;
  visionShots: VisionShot[];
  perception: Perception;

  shotCategory?: string;
}
interface VisionShot {
  name: string;
  categories?: { name: string }[];
}

function orEmpty(s?: string) {
  if (s == null)
    return '';
  return s;
}

export class MazeView extends Component<{}, MazeState> {
  constructor(props: any) {
    super(props);
    this.state =
      {
        game: "empire",
        is_grid_display: false,
        //img_name: "171213.233335.png",
        //img_name: "171213.233408.png",
        selectedVisionShot: {
          name: "171213.233408",
        },
        visionShots: [],
        perception: {
          frame: {
            x: 0, y: 0, width: 900, height: 1600
          },
          perceptionShots: []
        }
      };

    this.loadAll();

  }
  async loadAll() {
    await this.loadSetting();
    await Promise.all([this.load(), this.loadPerception()]);
  }

  async loadSetting() {
    let response = await fetch('data/setting.json');
    let setting = (await response.json());

    this.setState({ selectedVisionShot: { name: setting.selectedVisionShot }, game: setting.game });
  }
  async load() {
    let response = await fetch(this.state.game + '/visionShots');
    let visionShotFilenames = (await response.json()) as string[];
    let visionShots = visionShotFilenames.map(filename => ({ name: filename.replace(".png", ""), filename: filename }));

    this.setState({ visionShots: visionShots });
  }
  async loadShot(shot?: string) {
    if (shot == null)
      return;

    let response = await axios.get(`/shot/${shot}`);

    this.setState({ selectedVisionShot: { ...this.state.selectedVisionShot, ...response.data } });
  }

  async loadPerception() {
    let response = await fetch('data/' + this.state.game + '.perception.json');
    let perception = (await response.json()) as Perception;
    this.setState({ perception: perception });
  }
  async captureScreenshot() {
    let response = await this.post(this.state.game + '/screenshot/capture', {});
    let answer = (await response.json());
    this.setState({ selectedVisionShot: { name: answer.visionShot } });
    await this.load();
  }
  async waitCaptureScreenshot() {
    let response = await this.post(this.state.game + '/wait-capture', {});
    let answer = (await response.json());
    this.setState({ selectedVisionShot: { name: answer.visionShot } });
    await this.load();
  }

  async tap(p: { x: number, y: number }) {
    let response = await axios.post(this.state.game + '/tap', { x: Math.round(p.x), y: Math.round(p.y) });
    let answer = (await response.data);
  }
  async tapCapture(p: { x: number, y: number }) {
    let response = await axios.post(this.state.game + '/tap-capture', { x: Math.round(p.x), y: Math.round(p.y) });
    let answer = (await response.data);

    this.setState({ selectedVisionShot: { name: answer.visionShot } });
    await this.load();
  }
  async recognizeText(name: string) {
    let response = await this.post(this.state.game + '/screenshot/' + name + '/ocr-perception', {});
    let answer = (await response.json()) as PerceptionShot;
    this.setState({ perception: { ...this.state.perception, perceptionShots: this.state.perception.perceptionShots.concat([answer]) } });
  }
  async post(url: string, data: any) {
    return await fetch(url, { method: 'POST', body: JSON.stringify(data), headers: { 'Content-Type': 'application/json' } });
  }
  tapHandle = async (e: React.MouseEvent<HTMLImageElement>, k: number) => {
    //console.log(e.clientX, e.clientY);
    //console.log(e.currentTarget.x, e.currentTarget.y);

    const imgX = e.clientX - e.currentTarget.x;
    const imgY = e.clientY - e.currentTarget.y;

    const x = imgX / k;
    const y = imgY / k;

    console.log(x, y);
    await this.tapCapture({ x:x, y:y });
  }

  async applyCategory(shot: string, category?: string) {
    if (category == null)
      return;

    let response = await axios.put(`/shot/${shot}/category`, { category:category });
    let answer = (await response.data);

    await this.loadShot(this.state.selectedVisionShot.name);
  }

  public render() {
    //let visionShotFilenames = ["171213.233408.png", "171213.233335.png", "171213.233229.png"];
    //let visionShots = visionShotFilenames.map(filename => ({ name: filename.replace(".png", ""), filename: filename }));
    let visionShots = this.state.visionShots;

    let maze = this.state.perception;

    let visionFrame = maze.perceptionShots.find(shot => shot.shotName == this.state.selectedVisionShot.name);

    let resizeK = 0.4;
    let frame = MazeView.ResizeFrame(maze.frame, resizeK);
    let areas = MazeView.ResizeAreas(visionFrame == null ? [] : visionFrame.areas, resizeK);
    let grid = MazeView.ResizeRects(MazeView.Grid(maze.frame.width, maze.frame.height, 200), resizeK);
    //console.log(maze);

    return <div>
      <h1>{this.state.game}</h1>
      <div className="row">
        <div className="col-sm-2" style={{ height: "900px", overflowY: "scroll" }}>
          {
            visionShots.map((shot, k) =>
              <div key={k} style={{ color: shot.name == this.state.selectedVisionShot.name ? "blue" : "inherit", cursor: "pointer" }} onClick={() => { this.selectVisionShot(shot) }}>
                {shot.name}
              </div>
            )
          }
        </div>
        <div className="col-sm-10">
          <Button onClick={() => { this.toggleGridDisplay() }}>grid</Button>{' '}
          <Button onClick={() => { this.captureScreenshot() }}>capture</Button>{' '}
          <Button onClick={() => { this.waitCaptureScreenshot() }}>wait-capture</Button>{' '}
          <Button onClick={() => { this.recognizeText(this.state.selectedVisionShot.name) }}>recognize text</Button>{' '}
          <Input style={{ width: '100px', display:'inline-block' }} name='shotCategory' value={orEmpty(this.state.shotCategory)} onChange={this.handleChange} />{' '}
          <Button onClick={() => { this.applyCategory(this.state.selectedVisionShot.name, this.state.shotCategory) }}>apply category</Button>{' '}
          {optional(this.state.selectedVisionShot).k('categories').getOrElse([]).map((category, k) => <span key={k}>{`${optional(category).k('name').get()} `}</span>)}

          <div style={{ display: 'table-row' }}>
            <div style={{ width: frame.width, display: 'table-cell', textAlign: 'center', fontSize: '150%' }}>AI-Gamer Vision
                        </div>
            <div style={{ width: '20px', display: 'table-cell' }}></div>
            <div style={{ width: frame.width, display: 'table-cell', textAlign: 'center', fontSize: '150%' }}>AI-Gamer Perception</div>
            <div style={{ width: '20px', display: 'table-cell' }}></div>
            <div style={{ width: frame.width, display: 'table-cell', textAlign: 'center', fontSize: '150%' }}>AI-Gamer Mind</div>
          </div>
          <div style={{ display: 'table-row' }}>
            <div style={{ width: frame.width, height: frame.height, border: '0px solid black', display: 'table-cell', position: 'relative', color: 'lightblue' }}>
              <img style={{ width: frame.width, height: frame.height, position: 'absolute' }} src={this.state.game + '/screenshot/' + this.state.selectedVisionShot.name} onClick={e => this.tapHandle(e, resizeK)} />
              {
                this.state.is_grid_display ?
                  grid.map((area, k) =>
                    <div style={{ left: area.x, top: area.y, width: area.width, height: area.height, position: 'absolute', border: '1px dashed lightblue' }} key={k}>
                    </div>
                  )
                  : null
              }
              {areas.map((area, k) =>
                <div className="maze-cell" style={{ left: area.x, top: area.y, width: area.width, maxWidth: area.width, height: area.height, position: 'absolute', border: (MazeView.IsFloat(area.name) ? '2px solid red' : '0.5px solid lightpink'), zIndex: MazeView.IsFloat(area.name) ? 10 : 0 }} key={k}>
                  <div className="maze-text" style={{ left: area.x, top: area.y, width: area.width, maxWidth: area.width, height: area.height }} title={area.value} > {area.name}</div>
                </div>
              )
              }
            </div>
            <div style={{ width: '20px', display: 'table-cell' }}></div>
            <div style={{ width: frame.width, height: frame.height, border: '1px solid black', display: 'table-cell', position: 'relative', color: 'green' }}>
              {areas.map((area, k) =>
                <div className="maze-cell" style={{ left: area.x, top: area.y, width: area.width, maxWidth: area.width, height: area.height, position: 'absolute', border: (MazeView.IsFloat(area.name) ? '2px solid black' : '0.5px solid darkgray'), zIndex: MazeView.IsFloat(area.name) ? 10 : 0 }} key={k}>
                  <div className="maze-text" style={{ left: area.x, top: area.y, width: area.width, maxWidth: area.width, height: area.height }} title={area.value}>{area.value}</div>
                </div>
              )
              }
            </div>
          </div>
        </div>
      </div>

    </div>;
  }
  handleChange = (event: React.ChangeEvent<HTMLInputElement>, sourceName?: string) => {
    const { target } = event;
    let value: string | (string | undefined)[] = target.value;

    const name = target.name as keyof MazeState;

    const request = {
      [name]: value
    };

    this.setState(request as any);
  }

  static IsFloat(name: string | undefined): boolean {
    return name != null && !name.startsWith('cell-');
  }
  static Grid(width: number, height: number, size: number): Rect[] {
    return Array.from(Array(Math.floor((height - 1) / size - 1)).keys()).map(j => ({ x: 0, y: (j + 1) * size, width: width, height: size }))
      .concat(
        Array.from(Array(Math.floor((width - 1) / size - 1)).keys()).map(i => ({ x: (i + 1) * size, y: 0, width: size, height: height }))
      );
  }
  static ResizeAreas(areas: Area[], k: number): Area[] {
    return areas.map(area => {
      return {
        x: k * area.x,
        y: k * area.y,
        width: k * area.width,
        height: k * area.height,
        name: area.name,
        value: area.value
      };
    });
  }
  static ResizeFrame(rect: Rect, k: number): Rect {
    return {
      x: k * rect.x,
      y: k * rect.y,
      width: k * rect.width,
      height: k * rect.height,
    };
  }
  static ResizeRects(rects: Rect[], k: number): Rect[] {
    return rects.map(rect => {
      return {
        x: k * rect.x,
        y: k * rect.y,
        width: k * rect.width,
        height: k * rect.height,
      };
    })
  }
  toggleGridDisplay() {
    this.setState({
      is_grid_display: !this.state.is_grid_display
    });
  }
  selectVisionShot(shot: VisionShot) {
    this.setState({
      selectedVisionShot: shot
    });

    this.loadShot(optional(shot).k('name').get());
  }


  //incrementCounter() {
  //    this.setState({
  //        currentCount: this.state.currentCount + 1
  //    });
  //}
}

interface Rect {
    x: number;
    y: number;
    width: number;
    height: number;
}

interface Perception {
    frame: Rect;
    perceptionShots: PerceptionShot[];
}
interface PerceptionShot {
    shotName: string;
    areas: Area[];
}
interface Area extends Rect {
  name?: string;
  value?: string;
}
